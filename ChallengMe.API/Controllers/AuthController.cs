using ChallengMe.Models.Auth.DTOs;
using ChallengMe.Services.AuthService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace ChallengMe.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpPost("login-microsoft")]
        public async Task<IActionResult> LoginMicrosoft([FromBody] TokenMicrosoftRequest request)
        {
            var token = await _authService.LogingMicrosoftAsync(request.Code);
            return Ok(new AuthResponse { Token = token });
        }

        [AllowAnonymous]
        [HttpPost("login-email")]
        [EnableRateLimiting("login")]
        public async Task<IActionResult> LoginEmail([FromBody] LoginEmailRequest request)
        {
            var token = await _authService.LoginEmailAsync(request.Email, request.Password);
            return Ok(new AuthResponse { Token = token });
        }

        [AllowAnonymous]
        [HttpPost("registro")]
        [EnableRateLimiting("registro")]
        public async Task<IActionResult> Registro([FromBody] RegistroRequest request)
        {
            var token = await _authService.RegistroEmailAsync(
                request.Email,
                request.Password,
                request.NombreUsuario
            );
            return Created(string.Empty, new AuthResponse { Token = token });
        }

        [AllowAnonymous]
        [HttpPost("recuperar-password")]
        public async Task<IActionResult> RecuperarPassword([FromBody] RecuperarPasswordRequest request)
        {
            // TODO: Implementar RecuperarPasswordAsync en AuthService
            throw new NotImplementedException();
        }
    }
}
