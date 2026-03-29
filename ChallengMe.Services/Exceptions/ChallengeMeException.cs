using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChallengMe.Services.Exceptions
{
    public class ChallengeMeException : Exception
    {
        public int StatusCode { get; set; }

        public ChallengeMeException(string message, int statusCode) : base (message)
        {
            StatusCode = statusCode;
        }
    }
}
