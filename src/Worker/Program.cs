using Serilog;
using Worker.Configurations;

var builder = Host.CreateApplicationBuilder(args);

SerilogConfiguration.ConfigurarSerilog(builder.Configuration);
builder.Services.AddSerilog(Log.Logger, dispose: true);

builder.Services.AddDatabase(builder.Configuration);
builder.Services.AddMonitoring();
builder.Services.AddMessaging(builder.Configuration);
builder.Services.AddS3(builder.Configuration);
builder.Services.AddLlmServices(builder.Configuration);

var host = builder.Build();

host.AplicarMigracoes();

await host.RunAsync();

public partial class Program
{
    protected Program() { }
}