using ChallengMe.Web.Models.Auth;

namespace ChallengMe.Web.Services
{
    // AuthTokenHandler intercepta cada petición HTTP al API y añade el JWT en el header.
    // Lee el token del TokenStore, que fue cargado durante el prerender estático
    // desde la cookie HttpOnly por el componente InyectarToken.
    public class AuthTokenHandler : DelegatingHandler
    {
        private readonly TokenStore _tokenStore;

        public AuthTokenHandler(TokenStore tokenStore)
        {
            _tokenStore = tokenStore;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            var token = _tokenStore.Token;

            if (!string.IsNullOrEmpty(token))
                request.Headers.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            return await base.SendAsync(request, cancellationToken);
        }
    }
}