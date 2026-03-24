namespace ChallengMe.Web.Tools.Registrar
{
    public class RegistarTools
    {

        //Evaluar Registro
        public static Dictionary<string, string> ValidarRegistro(
        string nombreUsuario, string email, string password, string passwordConfirm)
        {
            var errores = new Dictionary<string, string>();

            if (string.IsNullOrWhiteSpace(nombreUsuario))
                errores["nombre"] = "El nombre de usuario es obligatorio.";
            else if (nombreUsuario.Length < 3)
                errores["nombre"] = "Mínimo 3 caracteres.";

            if (string.IsNullOrWhiteSpace(email))
                errores["email"] = "El email es obligatorio.";
            else if (!email.Contains("@"))
                errores["email"] = "Introduce un email válido.";

            if (string.IsNullOrWhiteSpace(password))
                errores["password"] = "La contraseña es obligatoria.";
            else if (password.Length < 8)
                errores["password"] = "Mínimo 8 caracteres.";

            if (password != passwordConfirm)
                errores["passwordConfirm"] = "Las contraseñas no coinciden.";

            return errores;
        }


        // Evaluar Contraseña
        public record Resultado(string Nivel, string Texto);

        public static Resultado Evaluar(string password)
        {
            if (string.IsNullOrEmpty(password))
                return new("", "");

            int puntos = 0;
            if (password.Length >= 8) puntos++;
            if (password.Any(char.IsUpper)) puntos++;
            if (password.Any(char.IsDigit)) puntos++;
            if (password.Any(ch => !char.IsLetterOrDigit(ch))) puntos++;

            return puntos switch
            {
                1 => new("debil", "Débil"),
                2 => new("media", "Media"),
                3 => new("buena", "Buena"),
                4 => new("fuerte", "Fuerte 💪"),
                _ => new("debil", "Débil")
            };
        }
    }
}
