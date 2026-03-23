using Infrastructure.Monitoramento.Correlation;
using NewRelic.LogEnrichers.Serilog;
using Serilog;

namespace Worker.Configurations;

public static class SerilogConfiguration
{
    private const string NomeServico = "ProcessamentoService";

    public static void ConfigurarSerilog(IConfiguration configuration)
    {
        var loggerConfig = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .Enrich.FromLogContext()
            .Enrich.With<CorrelationIdEnricher>()
            .Enrich.WithNewRelicLogsInContext()
            .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}");

        var licenseKey = configuration["NEW_RELIC_LICENSE_KEY"];
        var appName = configuration["NEW_RELIC_APP_NAME"] ?? NomeServico;
        var newRelicEndpoint = configuration["NEW_RELIC_LOG_ENDPOINT_URL"] ?? "https://log-api.newrelic.com/log/v1";

        if (!string.IsNullOrWhiteSpace(licenseKey))
            loggerConfig.WriteTo.NewRelicLogs(endpointUrl: newRelicEndpoint, applicationName: appName, licenseKey: licenseKey);

        Log.Logger = loggerConfig.CreateLogger();
    }
}
