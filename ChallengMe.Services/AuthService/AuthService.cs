using ChallengMe.AzureAD.AzureAd;
using ChallengMe.Models.Auth;
using ChallengMe.Services.JwtService;
using Microsoft.Extensions.Logging;
using ChallengMe.Models.Usuario;

namespace ChallengMe.Services.AuthService
{
    public class AuthService : IAuthService
    {
        private readonly IAzureAdService _azureAdService;
        //private readonly IUsuarioRepository _usuarioRepository;
        private readonly IJwtService _jwtService;
        private readonly ILogger<AuthService> _logger;

        public AuthService(IAzureAdService azureAdService, IJwtService jwtService, ILogger<AuthService> logger)
        {
            _azureAdService = azureAdService;
            _jwtService = jwtService;
            _logger = logger;
            //_usuarioRepository = usuarioRepository;
        }

        public async Task<AuthResultado> LoginMicrosoftAsync(string tokenMicrosoft)
        {
            
            var email = await _azureAdService.ValidarTokenYObtenerEmailAsync(tokenMicrosoft);
            if (email is null)
            {

            }


            var usuario = new Usuario();//await _usuarioRepository.ObtenerPorEmailAsync(email);
            if (usuario is null)
            {
                usuario = new Usuario
                {
                    Id = Guid.NewGuid(),
                    Email = email,
                    NombreUsuario = email.Split('@')[0],
                    PasswordHash = null,
                    FechaRegistro = DateTime.UtcNow
                };
                //await _usuarioRepository.CrearAsync(usuario);
            }

            
            var token = _jwtService.GenerarToken(usuario.Id, usuario.Email, usuario.NombreUsuario);
            return null; //AuthResultado.Ok(token);
        }
    }
}
