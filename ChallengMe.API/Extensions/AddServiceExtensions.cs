using ChallengMe.AzureAD.AzureAd;
using ChallengMe.Services.AuthService;
using ChallengMe.Services.JwtService;

namespace ChallengMe.API.Extensions
{
    public static class AddServiceExtensions
    {
        public static IServiceCollection AddServiceExtenions(this IServiceCollection services)
        {
            //Servidor de la autenticacion
            services.AddScoped<IJwtService, JwtService>();

            //Servidor de AzureAd
            services.AddHttpClient<IAzureAdService, AzureAdService>();

            //Servers
            services.AddScoped<IAuthService, AuthService>();

            return services;
        }
    }
}
