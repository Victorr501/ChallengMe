using ChallengMe.Services.AuthService;
using ChallengMe.Tests.API.Helpers;
using FluentAssertions;
using Moq;
using System.Net;
using System.Net.Http.Json;

namespace ChallengMe.Tests.API.Integration.Controllers
{
    public class AuthControllerRateLimitingTests : IClassFixture<RateLimitWebApplicationFactory>
    {
        private readonly HttpClient _client;
        private readonly Mock<IAuthService> _authServiceMock;

        public AuthControllerRateLimitingTests(RateLimitWebApplicationFactory factory)
        {
            _authServiceMock = factory.AuthServiceMock;
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task RateLimiting_MasDe3Solicitudes_Devuelve429()
        {
            _authServiceMock
                .Setup(s => s.SolicitarResetPasswordAsync(It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            var request = new { email = "spam@test.com" };

            for (int i = 0; i < 3; i++)
                await _client.PostAsJsonAsync("/api/auth/recuperar-password", request);

            var bloqueada = await _client.PostAsJsonAsync("/api/auth/recuperar-password", request);
            bloqueada.StatusCode.Should().Be(HttpStatusCode.TooManyRequests);
        }
    }
}