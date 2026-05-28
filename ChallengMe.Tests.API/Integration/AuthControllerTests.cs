using ChallengMe.Models.Auth.Response;
using ChallengMe.Services.AuthService;
using ChallengMe.Services.Exceptions.GenericExcepcions.Auth;
using ChallengMe.Tests.API.Helpers;
using FluentAssertions;
using Moq;
using System.Net;
using System.Net.Http.Json;

namespace ChallengMe.Tests.API.Integration.Controllers
{
    public class AuthControllerTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;
        private readonly Mock<IAuthService> _authServiceMock;

        public AuthControllerTests(CustomWebApplicationFactory factory)
        {
            _authServiceMock = factory.AuthServiceMock;
            _client = factory.CreateClient();
        }

        // ── LOGIN EMAIL ─────────────────────────────────────────────

        [Fact]
        public async Task LoginEmail_CredencialesCorrectas_Devuelve200ConToken()
        {
            _authServiceMock
                .Setup(s => s.LoginEmailAsync("test@test.com", "Password123"))
                .ReturnsAsync("token_valido");

            var response = await _client.PostAsJsonAsync("/api/auth/login-email",
                new { email = "test@test.com", password = "Password123" });

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var body = await response.Content.ReadFromJsonAsync<AuthResponse>();
            body!.Token.Should().Be("token_valido");
        }

        [Fact]
        public async Task LoginEmail_CredencialesIncorrectas_Devuelve401()
        {
            _authServiceMock
                .Setup(s => s.LoginEmailAsync("mal@test.com", "PasswordMal"))
                .ThrowsAsync(new CredencialesInvalidasException());

            var response = await _client.PostAsJsonAsync("/api/auth/login-email",
                new { email = "mal@test.com", password = "PasswordMal" });

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task LoginEmail_BodyVacio_Devuelve400()
        {
            var response = await _client.PostAsJsonAsync("/api/auth/login-email", new { });

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        // ── REGISTRO ────────────────────────────────────────────────

        [Fact]
        public async Task Registro_DatosNuevos_Devuelve201ConToken()
        {
            _authServiceMock
                .Setup(s => s.RegistroEmailAsync("nuevo@test.com", "Pass123", "NuevoUser"))
                .ReturnsAsync("token_registro");

            var response = await _client.PostAsJsonAsync("/api/auth/registro", new
            {
                email = "nuevo@test.com",
                password = "Pass123",
                nombreUsuario = "NuevoUser"
            });

            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var body = await response.Content.ReadFromJsonAsync<AuthResponse>();
            body!.Token.Should().Be("token_registro");
        }

        [Fact]
        public async Task Registro_EmailDuplicado_Devuelve409()
        {
            _authServiceMock
                .Setup(s => s.RegistroEmailAsync("duplicado@test.com", "Pass123", "User"))
                .ThrowsAsync(new EmailYaExisteException("duplicado@test.com"));

            var response = await _client.PostAsJsonAsync("/api/auth/registro", new
            {
                email = "duplicado@test.com",
                password = "Pass123",
                nombreUsuario = "User"
            });

            response.StatusCode.Should().Be(HttpStatusCode.Conflict);
        }

        // ── LOGIN MICROSOFT ─────────────────────────────────────────

        [Fact]
        public async Task LoginMicrosoft_TokenValido_Devuelve200ConToken()
        {
            _authServiceMock
                .Setup(s => s.LogingMicrosoftAsync("code_valido"))
                .ReturnsAsync("token_microsoft");

            var response = await _client.PostAsJsonAsync("/api/auth/login-microsoft",
                new { code = "code_valido" });

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var body = await response.Content.ReadFromJsonAsync<AuthResponse>();
            body!.Token.Should().Be("token_microsoft");
        }

        [Fact]
        public async Task LoginMicrosoft_TokenInvalido_Devuelve401()
        {
            _authServiceMock
                .Setup(s => s.LogingMicrosoftAsync("code_invalido"))
                .ThrowsAsync(new TokenMicrosoftInvalidoException());

            var response = await _client.PostAsJsonAsync("/api/auth/login-microsoft",
                new { code = "code_invalido" });

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        // ── RECUPERAR PASSWORD ──────────────────────────────────────

        [Fact]
        public async Task SolicitarReset_EmailValido_Devuelve200()
        {
            _authServiceMock
                .Setup(s => s.SolicitarResetPasswordAsync("victor@test.com"))
                .Returns(Task.CompletedTask);

            var response = await _client.PostAsJsonAsync("/api/auth/recuperar-password",
                new { email = "victor@test.com" });

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task SolicitarReset_EmailNoRegistrado_Devuelve200IgualQueValido()
        {
            // Seguridad por oscuridad: el cliente nunca sabe si el email existe o no
            _authServiceMock
                .Setup(s => s.SolicitarResetPasswordAsync("noexiste@test.com"))
                .Returns(Task.CompletedTask);

            var response = await _client.PostAsJsonAsync("/api/auth/recuperar-password",
                new { email = "noexiste@test.com" });

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
        

        // ── RESET PASSWORD ──────────────────────────────────────────

        [Fact]
        public async Task ResetPassword_TokenValido_Devuelve200()
        {
            _authServiceMock
                .Setup(s => s.ResetPasswordAsync("token-valido", "NuevoPass123"))
                .Returns(Task.CompletedTask);

            var response = await _client.PostAsJsonAsync("/api/auth/reset-password",
                new { token = "token-valido", nuevaPassword = "NuevoPass123" });

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async Task ResetPassword_TokenNoReutilizable_Devuelve401()
        {
            _authServiceMock
                .Setup(s => s.ResetPasswordAsync("token-ya-usado", "NuevoPass123"))
                .ThrowsAsync(new TokenResetInvalidoException());

            var response = await _client.PostAsJsonAsync("/api/auth/reset-password",
                new { token = "token-ya-usado", nuevaPassword = "NuevoPass123" });

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task ResetPassword_TokenExpirado_Devuelve401()
        {
            _authServiceMock
                .Setup(s => s.ResetPasswordAsync("token-expirado", "NuevoPass123"))
                .ThrowsAsync(new TokenResetExpiradoException());

            var response = await _client.PostAsJsonAsync("/api/auth/reset-password",
                new { token = "token-expirado", nuevaPassword = "NuevoPass123" });

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
    }
}