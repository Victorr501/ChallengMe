using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChallengMe.Services.Exceptions.GenericExcepcions.Auth
{
    public class TokenMicrosoftInvalidoException : AuthException
    {
        public TokenMicrosoftInvalidoException() : base("El token de Microsoft no es válido o ha expirado") { }
    }
}
