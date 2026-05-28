using ChallengMe.API.ExceptionMiddleware;
using ChallengMe.API.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddServiceExtenions(builder.Configuration);
builder.Services.AddRepositoryExtensions();
builder.Services.AddAuthServices(builder.Configuration);
builder.Services.AddConfigurationExtension(builder.Configuration);
builder.Services.AddSwaggerWithAuth();
builder.Services.AddRateLimitExtensions();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionMiddleware>();
app.UseHttpsRedirection();
app.UseConditionalMiddleware();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

public partial class Program { } // Para pruebas de integración