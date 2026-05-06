using ChallengMe.AzureAD.AzureAd;
using ChallengMe.Services.JwtService;
using Microsoft.Extensions.Logging;
using ChallengMe.Models.Usuario;
using ChallengMe.Repositories.UsuarioRepository;
using ChallengMe.Services.Exceptions.GenericExcepcions.Auth;
using ChallengMe.Services.Exceptions;

namespace ChallengMe.Services.AuthService
{
    public class AuthService : IAuthService
    {

        private readonly IAzureAdService _azureAdService;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IJwtService _jwtService;
        private readonly ILogger<AuthService> _logger;

        public AuthService(IAzureAdService azureAdService, IJwtService jwtService, ILogger<AuthService> logger, IUsuarioRepository usuarioRepository )
        {
            _azureAdService = azureAdService;
            _jwtService = jwtService;
            _logger = logger;
            _usuarioRepository = usuarioRepository;
        }

        public async Task<String> LogingMicrosoftAsync(string tokenMicrosoft)
        {
            try
            {
                var email = await _azureAdService.ValidarTokenYObtenerEmailAsync(tokenMicrosoft);
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
    }
}
