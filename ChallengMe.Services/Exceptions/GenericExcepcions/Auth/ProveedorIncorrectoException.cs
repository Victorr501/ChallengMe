using ChallengMe.Services.Exceptions.GenericExcepcions;

namespace ChallengMe.Services.Exceptions.GenericExcepcions.Auth
{
    public class ProveedorIncorrectoException : AuthException
    {
        /// <summary>
        /// HTTP 401 - El email existe pero fue registrado con un proveedor distinto.
        /// </summary>
        /// <param name="proveedor">Proveedor con el que se registró la cuenta: "email" | "microsoft"</param>
        public ProveedorIncorrectoException(string proveedor) : base(proveedor == "microsoft" 
            ? "Esta cuenta usa acceso con Microsoft. Usa el botón 'Iniciar sesión con Microsoft'."
            : "Esta cuenta usa email y contraseña. Usa el formulario de inicio de sesión.") {}
    }
}