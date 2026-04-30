using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using static System.Net.WebRequestMethods;
using System.Net.Http.Json;
using ChallengMe.Models.Auth.Response;

namespace ChallengMe.AzureAD.AzureAd
{
    public class AzureAdService : IAzureAdService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<IAzureAdService> _logger;
        private readonly HttpClient _http;

        public AzureAdService(IConfiguration config, ILogger<IAzureAdService> logger, HttpClient http)
        {
            _config = config;
            _logger = logger;
            _http = http;
        }

        public async Task<string?> ValidarTokenYObtenerEmailAsync(string tokenMicrosoft)
        {
            var tenantId = _config["AzureAd:TenantId"];
            var clientId = _config["AzureAd:ClientId"];
            var clientSecret = _config["AzureAd:ClientSecret"];
            var redirectUri = _config["AzureAd:RedirectUri"];

            _logger.LogInformation("Intercambiando authorization code por token");

            var tokenEndpoint = $"https://login.microsoftonline.com/{tenantId}/oauth2/v2.0/token";

            var formData = new Dictionary<string, string>
            {
                ["grant_type"] = "authorization_code",
                ["client_id"] = clientId!,
                ["client_secret"] = clientSecret!,
                ["code"] = tokenMicrosoft,
                ["redirect_uri"] = redirectUri!,
                ["scope"] = "openid profile email"
            };

            var response = await _http.PostAsync(
                tokenEndpoint,
                new FormUrlEncodedContent(formData));

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                _logger.LogCritical("Error al intercambiar el code: {Error}", error);
                return null;
            }

            var tokenResponse = await response.Content.ReadFromJsonAsync<TokenResponse>();
            var idToken = tokenResponse?.IdToken;

            if (string.IsNullOrWhiteSpace(idToken))
            {
                _logger.LogCritical("Microsoft no devolvió id_token");
                return null;
            }


            _logger.LogInformation("Validando id_token de Microsoft");

            try
            {
                var metadataUrl = $"https://login.microsoftonline.com/{tenantId}/v2.0/.well-known/openid-configuration";
                var configManager = new ConfigurationManager<OpenIdConnectConfiguration>(
                    metadataUrl,
                    new OpenIdConnectConfigurationRetriever());

                var openIdConfig = await configManager.GetConfigurationAsync();

                var validationParams = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKeys = openIdConfig.SigningKeys,
                    ValidateIssuer = true,
                    ValidIssuers = new[]
                    {
                    $"https://login.microsoftonline.com/{tenantId}/v2.0",
                    $"https://sts.windows.net/{tenantId}/"
                },
                    ValidateAudience = true,
                    ValidAudience = clientId,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

                var handler = new JwtSecurityTokenHandler();
                var principal = handler.ValidateToken(idToken, validationParams, out _);

                var email = principal.FindFirst("preferred_username")?.Value
                         ?? principal.FindFirst("email")?.Value;

                _logger.LogInformation("Token validado. Email: {Email}", email);
                return email;
            }
            catch (Exception ex)
            {
                _logger.LogCritical("Error al validar el id_token: {Message}", ex.Message);
                return null;
            }
        }
    }
}
