namespace ChallengMe.Web.Models.Auth
{
    public class LoginEmail
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool Recordarme { get; set; } = false;
    }
}
