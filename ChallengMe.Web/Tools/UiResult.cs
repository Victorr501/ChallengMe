namespace ChallengMe.Web.Tools
{
    public class UiResult
    {
        public bool Cargando { get; private set; }
        public bool Exitoso { get; private set; }
        public string Error { get; private set; } = string.Empty;
        public bool TieneError => !string.IsNullOrEmpty(Error);


        public static UiResult Inicial() => new();
        public static UiResult EnCarga() => new() { Cargando = true};
        public static UiResult Ok() => new() { Exitoso = true };
        public static UiResult ConError(string message) => new() { Error = message };
       
    }
}
