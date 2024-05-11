using LotusWebApp.Worker;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddSingleton<IHostedService, ConsumerService>();
var host = builder.Build();
host.Run();