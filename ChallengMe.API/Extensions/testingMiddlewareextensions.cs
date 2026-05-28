using ChallengMe.API.Extensions;

public static class TestingMiddlewareExtensions
{
    public static WebApplication UseConditionalMiddleware(this WebApplication app)
    {
        if (app.Configuration["DisableRateLimiting"] != "true")
            app.UseRateLimitExtensions();



        return app;
    }
}
