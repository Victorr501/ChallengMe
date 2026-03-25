using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Logging;


namespace ChallengMe.Services.JwtService
{
    public class JwtService: IJwtService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<JwtService> _logger;

        public JwtService(IConfiguration config, ILogger<JwtService> logger)
        {
            _config = config;
            _logger = logger;
        }

        public string GenerarToken(Guid usuarioId, string email, string nombreUsuario)
        {
            _logger.LogInformation("Se crea el token del usaurio con este {email}, con nombre {nombreUsuario}", email, nombreUsuario);
            var jwtSettings = _config.GetSection("Jwt");
            var secretKey = jwtSettings["SecretKey"]!;
            var issuer = jwtSettings["Issuer"]!;
            var audience = jwtSettings["Audience"]!;
            var expirationHours = int.Parse(jwtSettings["ExpirationHours"]!);


            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub,   usuarioId.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, email),
            new Claim("nombre",                      nombreUsuario),
            new Claim(JwtRegisteredClaimNames.Jti,   Guid.NewGuid().ToString())
            };


            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(expirationHours),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
