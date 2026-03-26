using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChallengMe.Services.Exceptions.GenericExcepcions
{
    public class AuthException : ChallengeMeException
    {
        /// <summary>
        /// HTTP 401 - credenciales inválidas, token expirado, cuenta bloqueada
        /// </summary>
        /// <param name="mensaje"></param>
        public AuthException(string mensaje) : base(mensaje, 401) { }
    }
}
