namespace ChallengMe.EmailServices.EmailServices
{
    public interface IEmailService
    {
        Task EnviarResetPasswordAsync(string email, string nombreUsuario, string token);
    }
}