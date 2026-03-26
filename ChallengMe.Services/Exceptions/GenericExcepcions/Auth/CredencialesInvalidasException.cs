using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChallengMe.Services.Exceptions.GenericExcepcions.Auth
{
    public class CredencialesInvalidasException : AuthException
    {
        /// <summary>
        /// El email o la contraseña son incorrectos.
        /// Mensaje genérico para no revelar cuál campo falla - US - 02
        /// </summary>
        public CredencialesInvalidasException() : base("Email o contraseña incorrectos") { }
    }
}
