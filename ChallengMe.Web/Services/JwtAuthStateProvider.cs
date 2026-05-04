using ChallengMe.Web.Models.API;
using Microsoft.AspNetCore.Components.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ChallengMe.Web.Services
{
    public class JwtAuthStateProvider : AuthenticationStateProvider
    {
        private readonly TokenStore _tokenStore;
        private readonly ILogger<JwtAuthStateProvider> _logger;

        private static readonly AuthenticationState _estadoAnonimo =
            new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));

        public JwtAuthStateProvider(TokenStore tokenStore, ILogger<JwtAuthStateProvider> logger)
        {
            _tokenStore = tokenStore;
            _logger = logger;
        }

        public override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = _tokenStore.Token;
            _logger.LogInformation($"[Auth] Token vacío: {string.IsNullOrEmpty(token)}");

            if (string.IsNullOrEmpty(token))
                return Task.FromResult(_estadoAnonimo);

            var claims = ExtraerClaims(token);
            if (claims == null)
                return Task.FromResult(_estadoAnonimo);

            var identity = new ClaimsIdentity(claims, "jwt");
            return Task.FromResult(new AuthenticationState(new ClaimsPrincipal(identity)));
        }

        public void EstablecerToken(string token)
        {
            _tokenStore.Token = token;
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }

        public void NotificarLogout()
        {
            _tokenStore.Token = null;
            NotifyAuthenticationStateChanged(Task.FromResult(_estadoAnonimo));
        }

        public string? ObtenerToken() => _tokenStore.Token;

        private static IEnumerable<Claim>? ExtraerClaims(string token)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);

                if (jwtToken.ValidTo < DateTime.UtcNow)
                    return null;

                return jwtToken.Claims;
            }
            catch
            {
                return null;
            }
        }
    }
}
