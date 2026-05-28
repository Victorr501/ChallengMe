using ChallengMe.EmailServices.EmailServices;
using ChallengMe.Models.Auth;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Net;
using Xunit;

namespace ChallengMe.Test.API.Unit.EmailServices;

public class EmailServiceTests
{


    private readonly Mock<ISendGridClient> _sendGridMock;
    private readonly Mock<ILogger<EmailService>> _loggerMock;
    private readonly EmailService _sut;  

    public EmailServiceTests()
    {
        _sendGridMock = new Mock<ISendGridClient>();
        _loggerMock = new Mock<ILogger<EmailService>>();

        var options = Options.Create(new SendGridOptions
        {
            ApiKey = "fake-api-key",
            EmailRemitente = "no-reply@challengme.com",
            NombreRemitente = "ChallengMe!"
        });

        _sut = new EmailService(options, _loggerMock.Object, _sendGridMock.Object);
    }


    [Fact]
    public async Task EnviarResetPasswordAsync_SendGridRespondeOk_LlamaEnviarConDatosCorrectos()
    {

        var respuestaFake = new Response(HttpStatusCode.Accepted, null, null);
        _sendGridMock
            .Setup(c => c.SendEmailAsync(It.IsAny<SendGridMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(respuestaFake);


        await _sut.EnviarResetPasswordAsync("victor@ejemplo.com", "Victor", "mi-token-123");


        _sendGridMock.Verify(
            c => c.SendEmailAsync(It.IsAny<SendGridMessage>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task EnviarResetPasswordAsync_SendGridRespondeOk_LoguearInformacion()
    {
        var respuestaFake = new Response(HttpStatusCode.Accepted, null, null);
        _sendGridMock
            .Setup(c => c.SendEmailAsync(It.IsAny<SendGridMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(respuestaFake);

        await _sut.EnviarResetPasswordAsync("victor@ejemplo.com", "Victor", "mi-token-123");

        _loggerMock.Verify(
            l => l.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, _) => v.ToString()!.Contains("victor@ejemplo.com")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task EnviarResetPasswordAsync_SendGridRespondeError_LoguearError()
    {
        var respuestaFake = new Response(HttpStatusCode.InternalServerError, null, null);
        _sendGridMock
            .Setup(c => c.SendEmailAsync(It.IsAny<SendGridMessage>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(respuestaFake);

        await _sut.EnviarResetPasswordAsync("victor@ejemplo.com", "Victor", "mi-token-123");

        _loggerMock.Verify(
            l => l.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, _) => v.ToString()!.Contains("victor@ejemplo.com")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task EnviarResetPasswordAsync_SendGridLanzaExcepcion_RelanzaLaExcepcion()
    {
        _sendGridMock
            .Setup(c => c.SendEmailAsync(It.IsAny<SendGridMessage>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new HttpRequestException("Fallo de red simulado"));

        await Assert.ThrowsAsync<HttpRequestException>(
            () => _sut.EnviarResetPasswordAsync("victor@ejemplo.com", "Victor", "mi-token-123"));
    }

    [Fact]
    public async Task EnviarResetPasswordAsync_SendGridLanzaExcepcion_LoguearError()
    {
        var excepcion = new HttpRequestException("Fallo de red simulado");
        _sendGridMock
            .Setup(c => c.SendEmailAsync(It.IsAny<SendGridMessage>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(excepcion);

        try { await _sut.EnviarResetPasswordAsync("victor@ejemplo.com", "Victor", "mi-token-123"); }
        catch { /* esperado */ }

        _loggerMock.Verify(
            l => l.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, _) => v.ToString()!.Contains("victor@ejemplo.com")),
                excepcion,
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}