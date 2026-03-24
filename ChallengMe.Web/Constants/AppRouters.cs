namespace ChallengMe.Web.Constants
{
    public static class AppRouters
    {
        public const string Landing = "/";
        public const string Login = "/login";
        public const string Registro = "/registro";
        public const string RecuperarPassword = "/recuperar-password";

        // Zona privada
        public static class App
        {
            public const string Dashboard = "/app/dashboard";
            public const string Reto = "/app/reto";
            public const string Histioral = "/app/historial";
            public const string Ranking = "/app/ranking";
            public const string Perfil = "/app/perfil";
        }
    }
}
