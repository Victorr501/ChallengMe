using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChallengMe.Services.Exceptions.GenericExcepcions
{
    /// <summary>
    /// HTTP 404 - recurso no encontrado en base de datos
    /// </summary>
    public class NotFoundException : ChallengeMeException
    {
        public NotFoundException(string recuro, string identificador) : 
            base ($"{recuro} '{identificador}' no encontrado.", 404) { }
    }
}
