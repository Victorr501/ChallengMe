using ChallengMe.AzureAD.AzureAd;
using ChallengMe.Repositories.DbConnectionFactory;
using ChallengMe.Services.JwtService;

namespace ChallengMe.API.Extensions
{
    public static class AddControllerExtensions
    {
        public static IServiceCollection AddControllerExtenions(this IServiceCollection services)
        {
            //Añadir conexion con la BD SQL
            services.AddScoped<IDbConnectionFactory, DbConnectionFactory>();


            return services;
        }
    }
}
