using ChallengMe.Web.Models.Auth;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ChallengMe.Web.Services
{
    public class JwtAuthStateProvider : AuthenticationStateProvider
    {
        private readonly IJSRuntime _js;
        private bool _inicializado = false;

        private static readonly AuthenticationState _estadoAnonimo =
            new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));

        public JwtAuthStateProvider(IJSRuntime js)
        {
            _js = js;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            // Durante el prerendering, devuelve anónimo sin redirigir
            if (!_inicializado)
                return _estadoAnonimo;

            try
            {
                var token = await _js.InvokeAsync<string?>(
                    "localStorage.getItem",
                    AuthConstants.JwtKey);

                if (string.IsNullOrEmpty(token))
                    return _estadoAnonimo;

                var claims = ExtraerClaims(token);
                if (claims == null)
                    return _estadoAnonimo;

                var identity = new ClaimsIdentity(claims, "jwt");
                return new AuthenticationState(new ClaimsPrincipal(identity));
            }
            catch (InvalidOperationException) { return _estadoAnonimo; }
            catch (JSException) { return _estadoAnonimo; }
            catch (TaskCanceledException) { return _estadoAnonimo; }
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

            NotifyAuthenticationStateChanged(
                Task.FromResult(new AuthenticationState(usuario)));
        }

        public void NotificarLogout()
        {
            NotifyAuthenticationStateChanged(
                Task.FromResult(_estadoAnonimo));
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