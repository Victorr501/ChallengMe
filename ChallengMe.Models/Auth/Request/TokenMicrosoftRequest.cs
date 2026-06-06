using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChallengMe.Models.Auth.DTOs
{
    public class TokenMicrosoftRequest
    {
        public string Code { get; set; }
        public string Plataforma { get; set; } = string.Empty;
    }
}
