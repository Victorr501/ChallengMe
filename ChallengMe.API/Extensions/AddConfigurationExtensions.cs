using ChallengMe.Models.Auth;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ChallengMe.API.Extensions
{
    public static class AddConfigurationExtensions
    {
        public static IServiceCollection AddConfigurationExtension(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.Configure<SendGridOptions>(
                configuration.GetSection("SendGrid"));

            return services;
        }
    }
}