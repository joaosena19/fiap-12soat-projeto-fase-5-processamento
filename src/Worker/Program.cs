using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using NewRelic.LogEnrichers.Serilog;
using Serilog;
using Worker.Configurations;

var builder = Host.CreateApplicationBuilder(args);
var configuration = builder.Configuration;

var loggerConfig = new LoggerConfiguration()
    .ReadFrom.Configuration(configuration)
    .Enrich.FromLogContext()
    .Enrich.With<Infrastructure.Monitoramento.Correlation.CorrelationIdEnricher>()
    .Enrich.WithNewRelicLogsInContext()
    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}");

var licenseKey = configuration["NEW_RELIC_LICENSE_KEY"];
var appName = configuration["NEW_RELIC_APP_NAME"] ?? "ProcessamentoService";
var newRelicEndpoint = configuration["NEW_RELIC_LOG_ENDPOINT_URL"] ?? "https://log-api.newrelic.com/log/v1";

if (!string.IsNullOrWhiteSpace(licenseKey))
{
    loggerConfig.WriteTo.NewRelicLogs(
        endpointUrl: newRelicEndpoint,
        applicationName: appName,
        licenseKey: licenseKey
    );
}

Log.Logger = loggerConfig.CreateLogger();
builder.Services.AddSerilog(Log.Logger, dispose: true);

builder.Services.AddDatabase(builder.Configuration);
builder.Services.AddMonitoring();
builder.Services.AddMessaging(builder.Configuration);
builder.Services.AddS3(builder.Configuration);
builder.Services.AddLlmServices(builder.Configuration);

var host = builder.Build();

using (var scope = host.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();
}

await host.RunAsync();

public partial class Program
{
    protected Program() { }
}