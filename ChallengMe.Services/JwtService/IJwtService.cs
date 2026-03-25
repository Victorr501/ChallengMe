using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChallengMe.Services.JwtService
{
    public interface IJwtService
    {
        string GenerarToken(Guid usuarioId, string email, string nombreUsuario);
    }
}
