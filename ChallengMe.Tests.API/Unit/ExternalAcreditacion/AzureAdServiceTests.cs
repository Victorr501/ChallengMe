using ChallengMe.AzureAD.AzureAd;
using ChallengMe.Tests.API.Helpers.Unit;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using System.Net;
using System.Text;
using System.Text.Json;

namespace ChallengMe.Tests.API.Unit.ExternalAcreditacion
{
    public class AzureAdServiceTests
    {
        private AzureAdService CrearServicio(HttpClient httpClient)
        {
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["AzureAd:TenantId"] = "tenant-test",
                    ["AzureAd:ClientId"] = "client-test",
                    ["AzureAd:ClientSecret"] = "secret-test",
                    ["AzureAd:RedirectUri"] = "https://localhost/callback"
                })
                .Build();

            return new AzureAdService(
                config,
                NullLogger<IAzureAdService>.Instance,
                httpClient);
        }

        // INTERCAMBIO DE CODE CON MICROSOFT

        [Fact]
        public async Task ValidarTokenYObtenerEmailAsync_MicrosoftRespondeError_DevuelveNull()
        {
            var httpClient = new HttpClient(
                new FakeHttpMessageHandler(HttpStatusCode.BadRequest, "invalid_grant"));

            var sut = CrearServicio(httpClient);

            var resultado = await sut.ValidarTokenYObtenerEmailAsync("code_invalido");

            resultado.Should().BeNull();
        }

        [Fact]
        public async Task ValidarTokenYObtenerEmailAsync_MicrosoftRespondeUnauthorized_DevuelveNull()
        {
            var httpClient = new HttpClient(
                new FakeHttpMessageHandler(HttpStatusCode.Unauthorized, "unauthorized_client"));

            var sut = CrearServicio(httpClient);

            var resultado = await sut.ValidarTokenYObtenerEmailAsync("code_cualquiera");

            resultado.Should().BeNull();
        }

        [Fact]
        public async Task ValidarTokenYObtenerEmailAsync_MicrosoftNoDevuelveIdToken_DevuelveNull()
        {
            var responseBody = JsonSerializer.Serialize(new
            {
                access_token = "access_token_de_prueba",
                token_type = "Bearer",
                expires_in = 3600
            });

            var httpClient = new HttpClient(
                new FakeHttpMessageHandler(HttpStatusCode.OK, responseBody));

            var sut = CrearServicio(httpClient);

            var resultado = await sut.ValidarTokenYObtenerEmailAsync("code_valido");

            resultado.Should().BeNull();
        }

        [Fact]
        public async Task ValidarTokenYObtenerEmailAsync_MicrosoftDevuelveIdTokenVacio_DevuelveNull()
        {
            var responseBody = JsonSerializer.Serialize(new
            {
                access_token = "access_token_de_prueba",
                token_type = "Bearer",
                expires_in = 3600,
                id_token = ""  
            });

            var httpClient = new HttpClient(
                new FakeHttpMessageHandler(HttpStatusCode.OK, responseBody));

            var sut = CrearServicio(httpClient);

            var resultado = await sut.ValidarTokenYObtenerEmailAsync("code_valido");

            resultado.Should().BeNull();
        }
    }
}