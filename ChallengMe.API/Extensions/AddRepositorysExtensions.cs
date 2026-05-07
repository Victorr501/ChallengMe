using ChallengMe.AzureAD.AzureAd;
using ChallengMe.Repositories.DbConnectionFactory;
using ChallengMe.Repositories.TokenResetPasswordRepository;
using ChallengMe.Repositories.UsuarioRepository;
using ChallengMe.Services.JwtService;

namespace ChallengMe.API.Extensions
{
    public static class AddRepositorysExtensions
    {
        public static IServiceCollection AddRepositoryExtensions(this IServiceCollection services)
        {
            //Añadir conexion con la BD SQL
            services.AddScoped<IDbConnectionFactory, DbConnectionFactory>();

            //Añadir repositorios
            services.AddScoped<IUsuarioRepository, UsuarioRepository>();
            services.AddScoped<ITokenResetPasswordRepository, TokenResetPasswordRepository>();

            return services;
        }
    }
}
