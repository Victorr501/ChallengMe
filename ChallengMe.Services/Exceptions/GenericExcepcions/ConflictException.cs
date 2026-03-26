using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChallengMe.Services.Exceptions.GenericExcepcions
{
    /// <summary>
    /// HTTP 409 - conflicto, recuros ya existe
    /// </summary>
    public class ConflictException : ChallengeMeException
    {
        public ConflictException(string mensaje): base (mensaje, 409) { }
    }
}
