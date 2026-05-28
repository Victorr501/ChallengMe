using ChallengMe.AzureAD.AzureAd;
using ChallengMe.EmailServices.EmailServices;
using ChallengMe.Services.AuthService;
using ChallengMe.Services.JwtService;
using SendGrid;

namespace ChallengMe.API.Extensions
{
    public static class AddServiceExtensions
    {
        public static IServiceCollection AddServiceExtenions(this IServiceCollection services, IConfiguration configuration)
        {
            //Servidor de la autenticacion
            services.AddScoped<IJwtService, JwtService>();

            //Servidor de AzureAd
            services.AddHttpClient<IAzureAdService, AzureAdService>();

            // Cliente de SendGrid para enviar emails
            services.AddSingleton<ISendGridClient>(sp =>
                new SendGridClient(configuration["SendGrid:ApiKey"]
                    ?? throw new InvalidOperationException("SendGrid:ApiKey no está configurado")));

            // Servidor de envio de email
            services.AddScoped<IEmailService, EmailService>();


            //Servers
            services.AddScoped<IAuthService, AuthService>();

            return services;
        }
    }
}
