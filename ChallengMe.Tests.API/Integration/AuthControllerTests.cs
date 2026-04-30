using ChallengMe.Models.Auth.Response;
using ChallengMe.Services.AuthService;
using ChallengMe.Services.Exceptions.GenericExcepcions.Auth;
using ChallengMe.Tests.API.Helpers;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
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

        // LOGIN EMAIL

        [Fact]
        public async Task LoginEmail_CredencialesCorrectas_Devuelve200ConToken()
        {

            _authServiceMock
                .Setup(s => s.LoginEmailAsync("test@test.com", "Password123"))
                .ReturnsAsync("token_valido");

            var request = new { email = "test@test.com", password = "Password123" };

            var response = await _client.PostAsJsonAsync("/api/auth/login-email", request);

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

            var request = new { email = "mal@test.com", password = "PasswordMal" };

            var response = await _client.PostAsJsonAsync("/api/auth/login-email", request);

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task LoginEmail_BodyVacio_Devuelve400()
        {
            var request = new { };

            var response = await _client.PostAsJsonAsync("/api/auth/login-email", request);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        // REGISTRO

        [Fact]
        public async Task Registro_DatosNuevos_Devuelve201ConToken()
        {
            _authServiceMock
                .Setup(s => s.RegistroEmailAsync("nuevo@test.com", "Pass123", "NuevoUser"))
                .ReturnsAsync("token_registro");

            var request = new
            {
                email = "nuevo@test.com",
                password = "Pass123",
                nombreUsuario = "NuevoUser"
            };

            var response = await _client.PostAsJsonAsync("/api/auth/registro", request);

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

            var request = new
            {
                email = "duplicado@test.com",
                password = "Pass123",
                nombreUsuario = "User"
            };

            var response = await _client.PostAsJsonAsync("/api/auth/registro", request);

            response.StatusCode.Should().Be(HttpStatusCode.Conflict);
        }

        // LOGIN MICROSOFT

        [Fact]
        public async Task LoginMicrosoft_TokenValido_Devuelve200ConToken()
        {
            _authServiceMock
                .Setup(s => s.LogingMicrosoftAsync("code_valido"))
                .ReturnsAsync("token_microsoft");

            var request = new { code = "code_valido" };

            var response = await _client.PostAsJsonAsync("/api/auth/login-microsoft", request);

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

            var request = new { code = "code_invalido" };

            var response = await _client.PostAsJsonAsync("/api/auth/login-microsoft", request);

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
    }
}