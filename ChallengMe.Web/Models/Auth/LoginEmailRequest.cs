namespace ChallengMe.Web.Models.Auth
{
    public class LoginEmailRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}