using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChallengMe.Services.Exceptions.GenericExcepcions.Auth
{
    /// <summary>
    /// El usuario intenta acceder sin haber verificado su email - US - 01
    /// </summary>
    public class EmailYaExisteException : ConflictException
    {
        public EmailYaExisteException(string email) : base($"El email '{email}' ya está registrado.") { }
    }
}
