using ChallengMe.Web.Models.Auth;
using Microsoft.JSInterop;

namespace ChallengMe.Web.Services
{
    public class AuthTokenHandler : DelegatingHandler
    {
        private readonly IJSRuntime _jSRuntime;

        public AuthTokenHandler(IJSRuntime jSRuntime)
        {
            _jSRuntime = jSRuntime;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            try
            {
                var token = await _jSRuntime.InvokeAsync<string?>("localStorage.getItem", cancellationToken, AuthConstants.JwtKey );
                if (!string.IsNullOrEmpty(token))
                {
                    request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                }
                
            }
            catch (InvalidOperationException)
            {

            } 
            catch (JSException)
            {

            }
            catch (TaskCanceledException)
            {

            }
            return await base.SendAsync(request, cancellationToken);
        }
    }
}
