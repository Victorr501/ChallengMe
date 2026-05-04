using ChallengMe.Web.Components;
using ChallengMe.Web.Extensions;
using ChallengMe.Web.Models.API;
using ChallengMe.Web.Services;
using Microsoft.AspNetCore.Components.Authorization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddHttpContextAccessor(); // ← esta línea debe estar

builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, JwtAuthStateProvider>();
builder.Services.AddScoped<TokenStore>();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddAuthentication();

// HttpClient con el handler del token (cookie + localStorage como fallback)
builder.Services.AddApiServices(builder.Configuration);


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




// ── Razor Components ──────────────────────────────────────────
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();