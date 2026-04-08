using ChallengMe.Web.Models.Auth;
using Microsoft.JSInterop;

namespace ChallengMe.Web.Services
{
    // AuthTokenHandler intercepta cada petición HTTP al API y añade el JWT en el header.
    // Estrategia dual:
    //   1. Primero intenta leer la cookie HttpOnly desde el HttpContext (Blazor Server, prod)
    //   2. Si no hay cookie, cae a localStorage via JS (desarrollo local con email/contraseña)
    public class AuthTokenHandler : DelegatingHandler
    {
        private readonly IJSRuntime _jsRuntime;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthTokenHandler(IJSRuntime jsRuntime, IHttpContextAccessor httpContextAccessor)
        {
            _jsRuntime = jsRuntime;
            _httpContextAccessor = httpContextAccessor;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var token = ObtenerTokenDeCookie();

            if (string.IsNullOrEmpty(token))
                token = await ObtenerTokenDeLocalStorageAsync(cancellationToken);

            if (!string.IsNullOrEmpty(token))
                request.Headers.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            return await base.SendAsync(request, cancellationToken);
        }

        // Lee la cookie HttpOnly directamente desde el servidor.
        // Esto funciona en Blazor Server porque el código corre en el servidor,
        // donde el HttpContext con las cookies del usuario está disponible.
        private string? ObtenerTokenDeCookie()
        {
            return _httpContextAccessor.HttpContext?.Request.Cookies[AuthConstants.JwtKey];
        }

        // Fallback para desarrollo local donde el login con email guarda en localStorage.
        private async Task<string?> ObtenerTokenDeLocalStorageAsync(CancellationToken cancellationToken)
        {
            try
            {
                return await _jsRuntime.InvokeAsync<string?>(
                    "localStorage.getItem",
                    cancellationToken,
                    AuthConstants.JwtKey);
            }
            catch (InvalidOperationException) { return null; }
            catch (JSException) { return null; }
            catch (TaskCanceledException) { return null; }
        }
    }
}