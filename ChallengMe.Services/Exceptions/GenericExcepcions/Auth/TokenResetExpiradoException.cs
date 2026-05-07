using ChallengMe.Services.Exceptions.GenericExcepcions;

namespace ChallengMe.Services.Exceptions.GenericExcepcions.Auth
{
    public class TokenResetExpiradoException : AuthException
    {
        /// <summary>
        /// HTTP 401 - El token de reset existe pero ha expirado.
        /// </summary>
        public TokenResetExpiradoException()
            : base("El enlace de recuperación ha expirado. Solicita uno nuevo.") { }
    }
}