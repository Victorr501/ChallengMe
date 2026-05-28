using ChallengMe.AzureAD.AzureAd;
using ChallengMe.EmailServices.EmailServices;
using ChallengMe.Models.Usuario;
using ChallengMe.Repositories.TokenResetPasswordRepository;
using ChallengMe.Repositories.UsuarioRepository;
using ChallengMe.Services.AuthService;
using ChallengMe.Services.Exceptions;
using ChallengMe.Services.Exceptions.GenericExcepcions.Auth;
using ChallengMe.Services.JwtService;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace ChallengMe.Tests.API.Unit.Services
{
    public class AuthServiceTests
    {
        private readonly Mock<IAzureAdService> _azureAdMock = new();
        private readonly Mock<IJwtService> _jwtMock = new();
        private readonly Mock<ILogger<AuthService>> _loggerMock = new();
        private readonly Mock<IUsuarioRepository> _repoMock = new();
        private readonly Mock<ITokenResetPasswordRepository> _tokenResetPasswordRepositoryMock = new();
        private readonly Mock<IEmailService> _emailServiceMock = new();
        private readonly AuthService _sut;

        public AuthServiceTests()
        {
            _sut = new AuthService(
                _azureAdMock.Object,
                _jwtMock.Object,
                _loggerMock.Object,
                _repoMock.Object,
                _tokenResetPasswordRepositoryMock.Object, 
                _emailServiceMock.Object);
        }

        //LoginEmailAsync

        [Fact]
        public async Task LoginEmailAsync_CredencialesCorrectas_DevuelveToken()
        {
            // ARRANGE
            var usuarioId = Guid.NewGuid();
            var hash = BCrypt.Net.BCrypt.HashPassword("Password123");

            var usuario = new Usuario
            {
                Id = usuarioId,
                Email = "test@test.com",
                NombreUsuario = "TestUser",
                PasswordHash = hash
            };

            _repoMock
                .Setup(r => r.ObtenerPorEmailAsync("test@test.com"))
                .ReturnsAsync(usuario);

            _jwtMock
                .Setup(j => j.GenerarToken(usuarioId, "test@test.com", "TestUser"))
                .Returns("token_valido");

            var resultado = await _sut.LoginEmailAsync("test@test.com", "Password123");

            resultado.Should().Be("token_valido");
        }

        [Fact]
        public async Task LoginEmailAsync_EmailNoExiste_LanzaCredencialesInvalidasException()
        {
            _repoMock
                .Setup(r => r.ObtenerPorEmailAsync("noexiste@test.com"))
                .ReturnsAsync((Usuario?)null);

            await _sut.Invoking(s => s.LoginEmailAsync("noexiste@test.com", "cualquier"))
                      .Should().ThrowAsync<CredencialesInvalidasException>();
        }

        [Fact]
        public async Task LoginEmailAsync_PasswordIncorrecta_LanzaCredencialesInvalidasException()
        {
            var usuario = new Usuario
            {
                Id = Guid.NewGuid(),
                Email = "test@test.com",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("PasswordCorrecta")
            };

            _repoMock
                .Setup(r => r.ObtenerPorEmailAsync("test@test.com"))
                .ReturnsAsync(usuario);

            await _sut.Invoking(s => s.LoginEmailAsync("test@test.com", "PasswordMal"))
                      .Should().ThrowAsync<CredencialesInvalidasException>();
        }

        [Fact]
        public async Task LoginEmailAsync_UsuarioGmailEntraConMicrosoft_LanzaProveedorIncorrectoException()
        {
            var usuario = new Usuario
            {
                Id = Guid.NewGuid(),
                Email = "microsoft@test.com",
                PasswordHash = null,
                ProveedorAutenticacion = "microsoft"
            };

            _repoMock
                .Setup(r => r.ObtenerPorEmailAsync("microsoft@test.com"))
                .ReturnsAsync(usuario);

            await _sut.Invoking(s => s.LoginEmailAsync("microsoft@test.com", "cualquier"))
                      .Should().ThrowAsync<ProveedorIncorrectoException>();
        }

        // RegistroEmailAsync

        [Fact]
        public async Task RegistroEmailAsync_EmailNuevo_CreaUsuarioYDevuelveToken()
        {
            _repoMock
                .Setup(r => r.ObtenerPorEmailAsync("nuevo@test.com"))
                .ReturnsAsync((Usuario?)null);

            _jwtMock
                .Setup(j => j.GenerarToken(It.IsAny<Guid>(), "nuevo@test.com", "NuevoUser"))
                .Returns("token_registro");


            var resultado = await _sut.RegistroEmailAsync("nuevo@test.com", "Pass123", "NuevoUser");

            resultado.Should().Be("token_registro");

            _repoMock.Verify(r => r.CrearAsync(It.IsAny<Usuario>()), Times.Once);
        }

        [Fact]
        public async Task RegistroEmailAsync_EmailDuplicado_LanzaEmailYaExisteException()
        {
            _repoMock
                .Setup(r => r.ObtenerPorEmailAsync("duplicado@test.com"))
                .ReturnsAsync(new Usuario { Email = "duplicado@test.com" });

            await _sut.Invoking(s => s.RegistroEmailAsync("duplicado@test.com", "Pass123", "User"))
                      .Should().ThrowAsync<EmailYaExisteException>();

            _repoMock.Verify(r => r.CrearAsync(It.IsAny<Usuario>()), Times.Never);
        }

        // LogingMicrosoftAsync

        [Fact]
        public async Task LogingMicrosoftAsync_TokenValido_UsuarioNuevo_CreaUsuarioYDevuelveToken()
        {
            _azureAdMock
                .Setup(a => a.ValidarTokenYObtenerEmailAsync("code_valido"))
                .ReturnsAsync("nuevo@microsoft.com");

            _repoMock
                .Setup(r => r.ObtenerPorEmailAsync("nuevo@microsoft.com"))
                .ReturnsAsync((Usuario?)null);

            _jwtMock
                .Setup(j => j.GenerarToken(It.IsAny<Guid>(), "nuevo@microsoft.com", "nuevo"))
                .Returns("token_microsoft_nuevo");

            var resultado = await _sut.LogingMicrosoftAsync("code_valido");

            resultado.Should().Be("token_microsoft_nuevo");

            _repoMock.Verify(r => r.CrearAsync(It.IsAny<Usuario>()), Times.Once);
        }

        [Fact]
        public async Task LogingMicrosoftAsync_TokenValido_UsuarioExistente_NoCreaNuevoUsuario()
        {
            var usuarioExistente = new Usuario
            {
                Id = Guid.NewGuid(),
                Email = "existente@microsoft.com",
                NombreUsuario = "existente",
                ProveedorAutenticacion = "microsoft"
            };

            _azureAdMock
                .Setup(a => a.ValidarTokenYObtenerEmailAsync("code_valido"))
                .ReturnsAsync("existente@microsoft.com");

            _repoMock
                .Setup(r => r.ObtenerPorEmailAsync("existente@microsoft.com"))
                .ReturnsAsync(usuarioExistente);

            _jwtMock
                .Setup(j => j.GenerarToken(usuarioExistente.Id, usuarioExistente.Email, usuarioExistente.NombreUsuario))
                .Returns("token_microsoft_existente");

            var resultado = await _sut.LogingMicrosoftAsync("code_valido");

            resultado.Should().Be("token_microsoft_existente");

            _repoMock.Verify(r => r.CrearAsync(It.IsAny<Usuario>()), Times.Never);
        }

        [Fact]
        public async Task LogingMicrosoftAsync_TokenInvalido_LanzaTokenMicrosoftInvalidoException()
        {

            _azureAdMock
                .Setup(a => a.ValidarTokenYObtenerEmailAsync("code_invalido"))
                .ReturnsAsync((string?)null);

            await _sut.Invoking(s => s.LogingMicrosoftAsync("code_invalido"))
                      .Should().ThrowAsync<TokenMicrosoftInvalidoException>();
        }
    }
}