using ChallengMe.Models.TokenResetPassword;

namespace ChallengMe.Repositories.TokenResetPasswordRepository
{
    public interface ITokenResetPasswordRepository
    {
        Task<TokenResetPassword?> ObtenerPorTokenAsync(string token);
        Task CrearAsync(TokenResetPassword tokenReset);
        Task MarcarComoUsadoAsync(Guid id);
        Task EliminarExpiradosAsync();
    }
}