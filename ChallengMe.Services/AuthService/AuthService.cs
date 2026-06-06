using ChallengMe.AzureAD.AzureAd;
using ChallengMe.EmailServices.EmailServices;
using ChallengMe.Models.TokenResetPassword;
using ChallengMe.Models.Usuario;
using ChallengMe.Repositories.TokenResetPasswordRepository;
using ChallengMe.Repositories.UsuarioRepository;
using ChallengMe.Services.Exceptions;
using ChallengMe.Services.Exceptions.GenericExcepcions.Auth;
using ChallengMe.Services.JwtService;
using Microsoft.Extensions.Logging;

namespace ChallengMe.Services.AuthService
{
    public class AuthService : IAuthService
    {

        private readonly IAzureAdService _azureAdService;
        private readonly IEmailService _emailService;
        private readonly IJwtService _jwtService;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly ITokenResetPasswordRepository _tokenResetPasswordRepository;
        private readonly ILogger<AuthService> _logger;

        public AuthService(IAzureAdService azureAdService, IJwtService jwtService, ILogger<AuthService> logger, IUsuarioRepository usuarioRepository, ITokenResetPasswordRepository tokenResetPasswordRepository, IEmailService emailService)
        {
            _azureAdService = azureAdService;
            _jwtService = jwtService;
            _emailService = emailService;
            _usuarioRepository = usuarioRepository;
            _tokenResetPasswordRepository = tokenResetPasswordRepository;
            _logger = logger;
        }

        public async Task<String> LogingMicrosoftAsync(string tokenMicrosoft, string plataforma)
        {
            try
            {
                var email = await _azureAdService.ValidarTokenYObtenerEmailAsync(tokenMicrosoft, plataforma);
                if (email is null)
                {
                    _logger.LogWarning("Token de Microsoft invalido");
                    throw new TokenMicrosoftInvalidoException();
                }


                var usuario = await _usuarioRepository.ObtenerPorEmailAsync(email);
                if (usuario is null)
                {
                    usuario = new Usuario
                    {
                        Id = Guid.NewGuid(),
                        Email = email,
                        NombreUsuario = email.Split('@')[0],
                        PasswordHash = null,
                        FechaRegistro = DateTime.UtcNow,
                        ProveedorAutenticacion = "microsoft",
                        EmailVerificado = true
                    };
                    await _usuarioRepository.CrearAsync(usuario);
                    // Aqui hay que añadir un metodo para actualizar el id si coincide de nuevo
                }
                else if (usuario.ProveedorAutenticacion != "microsoft")
                {
                    _logger.LogWarning("El email {email} ya esta registrado con otro proveedor de autenticacion", email);
                    throw new ProveedorIncorrectoException("email");
                }


                var token = _jwtService.GenerarToken(usuario.Id, usuario.Email, usuario.NombreUsuario);
                return token;
            }
            catch (ChallengeMeException)
            {
                throw;  
            }
            catch (Exception ex) 
            {
                _logger.LogWarning("Error en el servidor");
                throw new ChallengeMeException("Error en el servidor", 500);
            }
        }

        public async Task<String> RegistroEmailAsync(string email, string password, string nombreUsuario)
        {
            try
            {
                var usuarioExistente = await _usuarioRepository.ObtenerPorEmailAsync(email);
                if (usuarioExistente != null)
                {
                    _logger.LogWarning("El email ya esta registrado: {email}", email);
                    throw new EmailYaExisteException(email);
                }

                var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

                var usuario = new Usuario
                {
                    Id = Guid.NewGuid(),
                    Email = email,
                    NombreUsuario = nombreUsuario,
                    PasswordHash = passwordHash,
                    FechaRegistro = DateTime.UtcNow,
                    ProveedorAutenticacion = "email",
                    EmailVerificado = true
                };

                _logger.LogInformation("Creando nuevo usuario con email: {email}", email);
                await _usuarioRepository.CrearAsync(usuario);

                var token = _jwtService.GenerarToken(usuario.Id, usuario.Email, usuario.NombreUsuario);
                return token;
            }
            catch (ChallengeMeException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Error en el servidor");
                throw new ChallengeMeException("Error en el servidor", 500);
            }
        }

        public async Task<string> LoginEmailAsync(string email, string password)
        {
            var usuario = await _usuarioRepository.ObtenerPorEmailAsync(email);

            if (usuario is null)
            {
                _logger.LogWarning("Credenciales invalidas para email: {email}", email);
                throw new CredencialesInvalidasException();
            }

            if (usuario.ProveedorAutenticacion != "email")
            {
                _logger.LogWarning("El email {email} esta registrado con otro proveedor de autenticacion", email);
                throw new ProveedorIncorrectoException("microsoft");
            }

            
            if (usuario.PasswordHash == null)
            {
                _logger.LogWarning("Credenciales invalidas para email: {email}", email);
                throw new CredencialesInvalidasException();
            }


            var passwordCorrecta = BCrypt.Net.BCrypt.Verify(password, usuario.PasswordHash);
            if (!passwordCorrecta)
            {
                _logger.LogWarning("Credenciales invalidas para email: {email}", email);
                throw new CredencialesInvalidasException();
            }
                

            var token = _jwtService.GenerarToken(usuario.Id, usuario.Email, usuario.NombreUsuario);
            return token;
        }

        public async Task SolicitarResetPasswordAsync(string email)
        {
            try
            {
                var usuario = await _usuarioRepository.ObtenerPorEmailAsync(email);

                if (usuario is null || usuario.ProveedorAutenticacion != "email")
                {
                    _logger.LogWarning("Solicitud de reset para email no válido: {email}", email);
                    return;
                }

                var tokenReset = new TokenResetPassword
                {
                    Id = Guid.NewGuid(),
                    UsuarioId = usuario.Id,
                    Token = Guid.NewGuid().ToString("N"),
                    Expiracion = DateTime.UtcNow.AddHours(1),
                    Usado = false,
                    FechaCreacion = DateTime.UtcNow
                };

                await _tokenResetPasswordRepository.CrearAsync(tokenReset);
                await _emailService.EnviarResetPasswordAsync(usuario.Email, usuario.NombreUsuario, tokenReset.Token);

                _logger.LogInformation("Token de reset enviado a: {email}", email);
            }
            catch (ChallengeMeException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al solicitar reset de password para: {email}", email);
                throw new ChallengeMeException("Error en el servidor", 500);
            }
        }

        public async Task ResetPasswordAsync(string token, string nuevaPassword)
        {
            try
            {
                var tokenReset = await _tokenResetPasswordRepository.ObtenerPorTokenAsync(token);

                if (tokenReset is null || tokenReset.Usado)
                    throw new TokenResetInvalidoException();

                if (tokenReset.Expiracion < DateTime.UtcNow)
                    throw new TokenResetExpiradoException();

                var usuario = await _usuarioRepository.ObtenerPorIdAsync(tokenReset.UsuarioId);
                if (usuario is null)
                    throw new TokenResetInvalidoException();

                if (usuario.ProveedorAutenticacion != "email")
                    throw new ProveedorNoPermitePasswordException();

                usuario.PasswordHash = BCrypt.Net.BCrypt.HashPassword(nuevaPassword);
                await _usuarioRepository.ActualizarPasswordAsync(usuario.Id, usuario.PasswordHash);
                await _tokenResetPasswordRepository.MarcarComoUsadoAsync(tokenReset.Id);

                _logger.LogInformation("Password reseteado para usuario: {usuarioId}", usuario.Id);
            }
            catch (ChallengeMeException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al resetear password con token: {token}", token);
                throw new ChallengeMeException("Error en el servidor", 500);
            }
        }
    }
}
