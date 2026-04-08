using ChallengMe.Web.Models.Auth;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ChallengMe.Web.Services
{
    // IHttpContextAccessor nos da acceso al HttpContext desde Blazor Server.
    // En Blazor Server el código corre en el servidor, por lo que podemos leer
    // las cookies HttpOnly directamente sin pasar por JavaScript.
    public class JwtAuthStateProvider : AuthenticationStateProvider
    {
        private readonly IJSRuntime _js;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private bool _inicializado = false;

        private static readonly AuthenticationState _estadoAnonimo =
            new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));

        public JwtAuthStateProvider(IJSRuntime js, IHttpContextAccessor httpContextAccessor)
        {
            _js = js;
            _httpContextAccessor = httpContextAccessor;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            if (!_inicializado)
                return _estadoAnonimo;

            // 1. Intentar leer el token desde la cookie HttpOnly (flujo Microsoft y email en prod)
            var token = _httpContextAccessor.HttpContext?.Request.Cookies[AuthConstants.JwtKey];

            // 2. Si no hay cookie, intentar localStorage (flujo email en local con JS)
            if (string.IsNullOrEmpty(token))
            {
                try
                {
                    token = await _js.InvokeAsync<string?>("localStorage.getItem", AuthConstants.JwtKey);
                }
                catch (InvalidOperationException) { }
                catch (JSException) { }
                catch (TaskCanceledException) { }
            }

            if (string.IsNullOrEmpty(token))
                return _estadoAnonimo;

            var claims = ExtraerClaims(token);
            if (claims == null)
                return _estadoAnonimo;

            var identity = new ClaimsIdentity(claims, "jwt");
            return new AuthenticationState(new ClaimsPrincipal(identity));
        }

        public async Task InicializarAsync()
        {
            _inicializado = true;
            var authState = await GetAuthenticationStateAsync();
            NotifyAuthenticationStateChanged(Task.FromResult(authState));
        }

        public void NotificarLogin(string token)
        {
            var claims = ExtraerClaims(token) ?? [];
            var identidad = new ClaimsIdentity(claims, "jwt");
            var usuario = new ClaimsPrincipal(identidad);
            NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(usuario)));
        }

        public void NotificarLogout()
        {
            NotifyAuthenticationStateChanged(Task.FromResult(_estadoAnonimo));
        }

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