using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChallengMe.Models.Usuario
{
    public class Usuario
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string? PasswordHash { get; set; }
        public string NombreUsuario { get; set; } = string.Empty;
        public string? AvatarUrl { get; set; }
        public byte NivelDificultad { get; set; } = 1;
        public int PuntosTotal { get; set; } = 0;
        public int RachaActual { get; set; } = 0;
        public int RachaMaxima { get; set; } = 0;
        public DateTime? UltimaActividad { get; set; }
        public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;
        public string ProveedorAutenticacion { get; set; } = "email";
        public bool EmailVerificado { get; set; }
    }
}
