using ChallengMe.Web.Constants;
using ChallengMe.Web.Models.Auth;
using ChallengMe.Web.Services;

namespace ChallengMe.Web.Extensions;

public static class MicrosoftCallbackExtensions
{
    public static WebApplication MapMicrosoftCallback(this WebApplication app)
    {
        app.MapGet("/signin-oidc", async (HttpContext context, ApiClient api) =>
        {
            var code = context.Request.Query["code"].ToString();

            if (string.IsNullOrEmpty(code))
            {
                context.Response.Redirect("/login?error=microsoft");
                return;
            }

            try
            {
                var response = await api.Http.PostAsJsonAsync(
                    "/api/auth/login-microsoft",
                    new { Code = code });

                if (!response.IsSuccessStatusCode)
                {
                    context.Response.Redirect("/login?error=microsoft");
                    return;
                }

                var resultado = await response.Content.ReadFromJsonAsync<AuthResponse>();

                if (resultado?.Token is null)
                {
                    context.Response.Redirect("/login?error=microsoft");
                    return;
                }

                // Cookie HttpOnly: JS nunca puede leerla → inmune a XSS
                context.Response.Cookies.Append(AuthConstants.JwtKey, resultado.Token, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTimeOffset.UtcNow.AddHours(24)
                });

                context.Response.Redirect(AppRouters.Dashboard);
            }
            catch
            {
                context.Response.Redirect("/login?error=microsoft");
            }
        });

        return app;
    }
}