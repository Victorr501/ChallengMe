using ChallengMe.Web.Services;

namespace ChallengMe.Web.Extensions
{
    public static class HttpClientExtensions
    {
        public static IServiceCollection AddApiServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var apiBaseUrl = new Uri(configuration["ApiSettings:BaseUrl"]!);

            services.AddTransient<AuthTokenHandler>();

            void Configurar(IHttpClientBuilder b) => b
                .ConfigureHttpClient(c => c.BaseAddress = apiBaseUrl)
                .AddHttpMessageHandler<AuthTokenHandler>();

            // ── Aquí registras cada servicio nuevo que vayas creando ──
            Configurar(services.AddHttpClient<AuthApiService>());

            return services;
        }
    }
}