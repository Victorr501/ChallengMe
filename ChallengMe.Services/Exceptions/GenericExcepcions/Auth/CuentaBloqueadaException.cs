using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChallengMe.Services.Exceptions.GenericExcepcions.Auth
{
    public class CuentaBloqueadaException : AuthException
    {
        /// <summary>
        /// El usuario ha superado el límite de inetnos fallidos - US - 02
        /// </summary>
        public CuentaBloqueadaException() : base ("Cuenta bloqueada temporalmente por demasiados intentos fallidos") { }
    }
}
