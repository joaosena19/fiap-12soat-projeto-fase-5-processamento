using Microsoft.Extensions.Configuration;
using Serilog;
using Worker.Configurations;

namespace Tests.Worker.Configurations;

public class SerilogConfigurationTests
{
    [Fact(DisplayName = "Deve configurar logger mesmo sem licença do New Relic")]
    [Trait("Worker", "SerilogConfiguration")]
    public void ConfigurarSerilog_DeveConfigurarLogger_QuandoLicencaNaoInformada()
    {
        // Arrange
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>())
            .Build();

        // Act
        SerilogConfiguration.ConfigurarSerilog(configuration);

        // Assert — SilentLogger (padrão) retorna false para IsEnabled; logger configurado retorna true
        Log.Logger.ShouldNotBeNull();
        Log.Logger.IsEnabled(Serilog.Events.LogEventLevel.Information).ShouldBeTrue();
    }

    [Fact(DisplayName = "Deve configurar logger quando licença do New Relic é informada")]
    [Trait("Worker", "SerilogConfiguration")]
    public void ConfigurarSerilog_DeveConfigurarLogger_QuandoLicencaInformada()
    {
        // Arrange
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["NEW_RELIC_LICENSE_KEY"] = "license-key",
                ["NEW_RELIC_APP_NAME"] = "processamento-tests",
                ["NEW_RELIC_LOG_ENDPOINT_URL"] = "https://log-api.newrelic.com/log/v1"
            })
            .Build();

        // Act
        SerilogConfiguration.ConfigurarSerilog(configuration);

        // Assert — SilentLogger (padrão) retorna false para IsEnabled; logger configurado retorna true
        Log.Logger.ShouldNotBeNull();
        Log.Logger.IsEnabled(Serilog.Events.LogEventLevel.Information).ShouldBeTrue();
    }
}