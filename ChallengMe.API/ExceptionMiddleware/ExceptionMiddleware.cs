using ChallengMe.Services.Exceptions;
using System.Text.Json;

namespace ChallengMe.API.ExceptionMiddleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (ChallengeMeException ex)
            {
                await EscrbirRespuestaAsync(context, ex.StatusCode, ex.Message);
            }
            catch (Exception ex)
            {
                await EscrbirRespuestaAsync(context, 500, "Ha ocurrido un error interno");
            }
        }

        private static async Task EscrbirRespuestaAsync(
            HttpContext context, int statusCode, string message)
        {
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";

            var respuesta = JsonSerializer.Serialize(new { message });
            await context.Response.WriteAsync(respuesta);
        }
    }
}
