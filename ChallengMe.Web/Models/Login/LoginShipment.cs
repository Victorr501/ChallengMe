namespace ChallengMe.Web.Models.Login
{
    public class LoginShipment
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool Recordarme { get; set; } = false;
    }
}
