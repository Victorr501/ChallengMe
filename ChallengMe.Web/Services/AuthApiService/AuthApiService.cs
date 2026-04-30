using ChallengMe.Web.Models.Auth.Shipment;

namespace ChallengMe.Web.Services
{
    public class AuthApiService
    {
        private readonly HttpClient _http;

        public AuthApiService(HttpClient http)
        {
            _http = http;
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
                new AuthMicrosoftShipment { Code = code });

            if (!response.IsSuccessStatusCode)
                return null;

            return await response.Content.ReadFromJsonAsync<AuthShipment>();
        }
    }
}