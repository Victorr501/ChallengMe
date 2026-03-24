namespace ChallengMe.Web.Tools.Login
{
    public class LoginTools
    {
        public static Dictionary<string, string> ValidarLogin(string email, string password)
        {
            var errores = new Dictionary<string, string>();

            if (string.IsNullOrWhiteSpace(email))
                errores["email"] = "El email es obligatorio.";
            else if (!email.Contains("@"))
                errores["email"] = "Introduce un email válido.";

            if (string.IsNullOrWhiteSpace(password))
                errores["password"] = "La contraseña es obligatoria.";

            return errores;
        }
    }
}
