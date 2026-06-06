using ChallengMe.Web.Models.Auth.Request;
using ChallengMe.Web.Models.Auth.Shipment;

namespace ChallengMe.Web.Services
{
    public class AuthApiService
    {
        private readonly HttpClient _http;
        private readonly string _plataforma;

        public AuthApiService(HttpClient http, IConfiguration config)
        {
            _http = http;
            var baseUrl = config["ApiBaseUrl"] ?? "";
            _plataforma = baseUrl.Contains("localhost") ? "local" : "web";
        }

        public async Task<AuthShipment?> LoginEmailAsync(string email, string password)
        {
            var response = await _http.PostAsJsonAsync(
                "/api/auth/login-email",
                new LoginEmailRequest
                {
                    Email = email,
                    Password = password
                });

            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadFromJsonAsync<AuthShipment>();
        }

        public async Task<AuthShipment?> RegistroEmailAsync(
            string email, string password, string nombreUsuario)
        {
            var response = await _http.PostAsJsonAsync(
                "/api/auth/registro",
                new RegistroRequest
                {
                    Email = email,
                    Password = password,
                    NombreUsuario = nombreUsuario
                });

            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadFromJsonAsync<AuthShipment>();
        }

        public async Task<AuthShipment?> LoginMicrosoftAsync(string code)
        {
            var response = await _http.PostAsJsonAsync(
                "/api/auth/login-microsoft",
                new AuthMicrosoftShipment 
                { 
                    Code = code,
                    Plataforma = _plataforma
                });

            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadFromJsonAsync<AuthShipment>();
        }

        public async Task RecuperarPasswordAsync(string email)
        {
            await _http.PostAsJsonAsync(
                "/api/auth/recuperar-password",
                new { Email = email });
        }

        public async Task<bool> ResetPasswordAsync(string token, string nuevaPassword)
        {
            var response = await _http.PostAsJsonAsync(
                "/api/auth/reset-password",
                new { Token = token, NuevaPassword = nuevaPassword });

            return response.IsSuccessStatusCode;
        }
    }
}