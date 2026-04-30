using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChallengMe.Models.Auth.DTOs
{
    public class RecuperarPasswordRequest
    {
        public string Email { get; set; } = string.Empty;
    }
}
