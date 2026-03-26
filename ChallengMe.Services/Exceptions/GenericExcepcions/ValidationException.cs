using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChallengMe.Services.Exceptions.GenericExcepcions
{
    /// <summary>
    /// HTTP 400 - datos de entrada inválidos
    /// </summary>
    public class ValidationException : ChallengeMeException
    {
        public ValidationException(string mensaje): base(mensaje, 409) { }
    }
}
