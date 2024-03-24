using LotusWebApp;
using LotusWebApp.Data;
using Microsoft.EntityFrameworkCore;

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

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dataContext = scope.ServiceProvider.GetRequiredService<MainDbContext>();
    dataContext.Database.Migrate();
}
app.MapHealthChecks("/healthz")
    .RequireHost("*:8000");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/health", () => new
    {
        Status = "OK"
    })
    .WithName("GetHealth")
    .WithOpenApi();

app.MapControllers();
app.Run();