using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChallengMe.Models.Auth
{
    public class SendGridOptions
    {
        public string ApiKey { get; set; } = string.Empty;
        public string EmailRemitente { get; set; } = string.Empty;
        public string NombreRemitente { get; set; } = string.Empty;
    }

}
