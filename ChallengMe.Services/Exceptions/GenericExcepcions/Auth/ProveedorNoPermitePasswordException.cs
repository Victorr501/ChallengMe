using ChallengMe.Services.Exceptions.GenericExcepcions;

namespace ChallengMe.Services.Exceptions.GenericExcepcions.Auth
{
    public class ProveedorNoPermitePasswordException : AuthException
    {
        /// <summary>
        /// HTTP 401 - La cuenta usa un proveedor externo y no tiene contraseña propia.
        /// </summary>
        public ProveedorNoPermitePasswordException()
            : base("Esta cuenta usa acceso con Microsoft. No es posible cambiar la contraseña.") { }
    }
}