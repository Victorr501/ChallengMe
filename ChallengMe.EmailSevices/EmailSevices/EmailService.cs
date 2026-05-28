using ChallengMe.Models.Auth;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;


namespace ChallengMe.EmailServices.EmailServices
{
    public class EmailService : IEmailService
    {
        private readonly SendGridOptions _options;
        private readonly ILogger<EmailService> _logger;
        private readonly ISendGridClient _sendGridClient;

        public EmailService(IOptions<SendGridOptions> options, ILogger<EmailService> logger, ISendGridClient sendGridClient)
        {
            _options = options.Value;
            _logger = logger;
            _sendGridClient = sendGridClient;
        }

        public async Task EnviarResetPasswordAsync(string email, string nombreUsuario, string token)
        {
            var enlace = $"https://challengme-web-h9b6hmftgtc7cmch.spaincentral-01.azurewebsites.net/reset-password?token={token}";

            var asunto = "Recupera tu contraseña — ChallengMe!";

            var cuerpoHtml = $"""
                <div style="font-family: Arial, sans-serif; max-width: 480px; margin: 0 auto;">
                    <h2 style="color: #3B82F6;">ChallengMe!</h2>
                    <p>Hola <strong>{nombreUsuario}</strong>,</p>
                    <p>Hemos recibido una solicitud para restablecer tu contraseña.</p>
                    <p>Haz clic en el botón para crear una nueva contraseña. El enlace es válido durante <strong>1 hora</strong>.</p>
                    <a href="{enlace}" 
                       style="display: inline-block; background-color: #3B82F6; color: white;
                              padding: 12px 24px; border-radius: 8px; text-decoration: none;
                              font-weight: bold; margin: 16px 0;">
                        Restablecer contraseña
                    </a>
                    <p style="color: #94A3B8; font-size: 14px;">
                        Si no has solicitado este cambio, ignora este email. 
                        Tu contraseña seguirá siendo la misma.
                    </p>
                    <hr style="border-color: #334155;" />
                    <p style="color: #94A3B8; font-size: 12px;">ChallengMe! · Madrid, España</p>
                </div>
                """;

            await EnviarAsync(email, asunto, cuerpoHtml);
        }


        // Método privado para enviar el email utilizando SendGrid
        private async Task EnviarAsync(string destinatario, string asunto, string cuerpoHtml)
        {
            try
            {
                var from = new EmailAddress(_options.EmailRemitente, _options.NombreRemitente);
                var to = new EmailAddress(destinatario);
                var mensaje = MailHelper.CreateSingleEmail(from, to, asunto, null, cuerpoHtml);

                var respuesta = await _sendGridClient.SendEmailAsync(mensaje); // ← usa el inyectado

                if (!respuesta.IsSuccessStatusCode)
                    _logger.LogError("Error al enviar email a {destinatario}. StatusCode: {code}",
                        destinatario, respuesta.StatusCode);
                else
                    _logger.LogInformation("Email enviado correctamente a {destinatario}", destinatario);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Excepcion al enviar email a {destinatario}", destinatario);
                throw;
            }
        }
    }
}