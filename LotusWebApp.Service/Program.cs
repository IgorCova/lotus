using LotusWebApp;
using LotusWebApp.Data;
using LotusWebApp.Metrics;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Metrics;
using Prometheus;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Services.RegisterContext(typeof(IContextRegistration), configuration);
builder.Services.RegisterAllRepository();
builder.Services.RegisterDataProviderService();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks()
    .AddCheck<HealthCheck>("default");

builder.Services.AddOpenTelemetry();
var app = builder.Build();

// Мидлварь для сбора http request метрик
app.UseHttpRequestMetrics();

using (var scope = app.Services.CreateScope())
{
    var dataContext = scope.ServiceProvider.GetRequiredService<MainDbContext>();
    dataContext.Database.Migrate();
}
app.MapHealthChecks("/healthz")
    .RequireHost("*:8000");

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.MapGet("/health", () => new
    {
        Status = "OK"
    })
    .WithName("GetHealth")
    .WithOpenApi();

app.MapControllers();
app.UseRouting();
app.MapMetrics();
app.Run();