using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChallengMe.Models.Auth.DTOs
{
    public class RegistroRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string NombreUsuario { get; set; } = string.Empty;
    }
}
