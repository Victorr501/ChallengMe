using ChallengMe.Services.Exceptions.GenericExcepcions;

namespace ChallengMe.Services.Exceptions.GenericExcepcions.Auth
{
    public class TokenResetInvalidoException : AuthException
    {
        /// <summary>
        /// HTTP 401 - El token de reset no existe o ya fue usado.
        /// </summary>
        public TokenResetInvalidoException()
            : base("El enlace de recuperación no es válido o ya ha sido utilizado.") { }
    }
}