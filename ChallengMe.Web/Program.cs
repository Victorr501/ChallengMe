using ChallengMe.Web.Components;
using ChallengMe.Web.Extensions;
using ChallengMe.Web.Services;
using Microsoft.AspNetCore.Components.Authorization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Necesario para que AuthTokenHandler y JwtAuthStateProvider
// puedan leer las cookies HttpOnly desde el servidor
builder.Services.AddHttpContextAccessor();

builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, JwtAuthStateProvider>();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddAuthentication();

// HttpClient con el handler del token (cookie + localStorage como fallback)
builder.Services.AddTransient<AuthTokenHandler>();
builder.Services.AddHttpClient<ApiClient>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiSettings:BaseUrl"]!);
}).AddHttpMessageHandler<AuthTokenHandler>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthentication();
app.UseAntiforgery();

// ── Callback de Microsoft OAuth ───────────────────────────────
app.MapMicrosoftCallback();

// ── Razor Components ──────────────────────────────────────────
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();