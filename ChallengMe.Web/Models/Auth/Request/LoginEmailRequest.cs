namespace ChallengMe.Web.Models.Auth.Request
{
    public class LoginEmailRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}