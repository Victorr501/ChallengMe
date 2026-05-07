using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChallengMe.Models.TokenResetPassword
{
    public class TokenResetPassword
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UsuarioId { get; set; }
        public string Token { get; set; } = string.Empty;
        public DateTime Expiracion { get; set; }
        public bool Usado { get; set; } = false;
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    }
}