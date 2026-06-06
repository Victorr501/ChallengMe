using ChallengMe.Models.Auth.DTOs;
using ChallengMe.Models.Auth.Response;
using ChallengMe.Services.AuthService;
using Microsoft.AspNetCore.Authorization;
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
        [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> LoginMicrosoft([FromBody] TokenMicrosoftRequest request)
        {
            var token = await _authService.LogingMicrosoftAsync(request.Code, request.Plataforma);
            return Ok(new AuthResponse { Token = token });
        }

        [AllowAnonymous]
        [HttpPost("login-email")]
        [EnableRateLimiting("login")]
        [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> LoginEmail([FromBody] LoginEmailRequest request)
        {
            var token = await _authService.LoginEmailAsync(request.Email, request.Password);
            return Ok(new AuthResponse { Token = token });
        }

        [AllowAnonymous]
        [HttpPost("registro")]
        [EnableRateLimiting("registro")]
        [ProducesResponseType(typeof(AuthResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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
        [EnableRateLimiting("recuperar-password")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RecuperarPassword([FromBody] RecuperarPasswordRequest request)
        {
            await _authService.SolicitarResetPasswordAsync(request.Email);
            return Ok();
        }

        [AllowAnonymous]
        [HttpPost("reset-password")]
        [EnableRateLimiting("reset-password")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            await _authService.ResetPasswordAsync(request.Token, request.NuevaPassword);
            return Ok();
        }
    }
}