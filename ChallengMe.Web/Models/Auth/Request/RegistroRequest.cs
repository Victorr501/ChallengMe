namespace ChallengMe.Web.Models.Auth.Request
{
    public class RegistroRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string NombreUsuario { get; set; } = string.Empty;
    }
}
