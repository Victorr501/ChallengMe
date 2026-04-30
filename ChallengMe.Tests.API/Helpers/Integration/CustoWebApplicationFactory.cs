// Helpers/CustomWebApplicationFactory.cs
using ChallengMe.Services.AuthService;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace ChallengMe.Tests.API.Helpers
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        public Mock<IAuthService> AuthServiceMock { get; } = new();

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(IAuthService));

                if (descriptor != null)
                    services.Remove(descriptor);

                services.AddScoped<IAuthService>(_ => AuthServiceMock.Object);
            });

            builder.UseEnvironment("Development");
        }
    }
}