using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Logging;

namespace ChallengMe.AzureAD.AzureAd
{
    public class AzureAdService : IAzureAdService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<IAzureAdService> _logger;

        public AzureAdService(IConfiguration config, ILogger<IAzureAdService> logger)
        {
            _config = config;
            _logger = logger;
        }

        public async Task<string?> ValidarTokenYObtenerEmailAsync(string tokenMicrosoft)
        {
            try
            {
                var tenantId = _config["AzureAd:TenantId"];
                var clientId = _config["AzureAd:ClientId"];

                _logger.LogInformation("Se esta verificando el token de microsoft");
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
                var principal = handler.ValidateToken(tokenMicrosoft, validationParams, out _);


                var email = principal.FindFirst("preferred_username")?.Value
                         ?? principal.FindFirst("email")?.Value;

                return email;
            }
            catch
            {
                _logger.LogCritical("Error al verificar el nuevo token");
                return null;
            }
        }
    }
}
