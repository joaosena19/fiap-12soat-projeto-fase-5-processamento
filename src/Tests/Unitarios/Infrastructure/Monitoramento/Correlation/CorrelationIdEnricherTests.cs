using Infrastructure.Monitoramento.Correlation;
using Serilog.Core;
using Serilog.Events;
using Serilog.Parsing;

namespace Tests.Infrastructure.Monitoramento.Correlation;

public class CorrelationIdEnricherTests
{
    [Fact(DisplayName = "Deve adicionar propriedade de correlation id quando contexto possui valor")]
    [Trait("Infrastructure", "CorrelationIdEnricher")]
    public void Enrich_DeveAdicionarPropriedade_QuandoCorrelationIdPresenteNoContexto()
    {
        // Arrange
        var enricher = new CorrelationIdEnricher();
        var evento = CriarEventoLog();
        var factory = new PropertyFactoryStub();
        var correlationId = Guid.NewGuid().ToString();

        // Act
        using (CorrelationContext.Push(correlationId))
            enricher.Enrich(evento, factory);

        // Assert
        evento.Properties.ContainsKey(CorrelationConstants.LogPropertyName).ShouldBeTrue();
        evento.Properties[CorrelationConstants.LogPropertyName].ToString().Trim('"').ShouldBe(correlationId);
    }

    [Fact(DisplayName = "Não deve sobrescrever propriedade quando correlation id já existe")]
    [Trait("Infrastructure", "CorrelationIdEnricher")]
    public void Enrich_NaoDeveSobrescreverPropriedade_QuandoJaExistirNoEvento()
    {
        // Arrange
        var enricher = new CorrelationIdEnricher();
        var evento = CriarEventoLog();
        evento.AddPropertyIfAbsent(new LogEventProperty(CorrelationConstants.LogPropertyName, new ScalarValue("valor-existente")));
        var factory = new PropertyFactoryStub();

        // Act
        using (CorrelationContext.Push("valor-novo"))
            enricher.Enrich(evento, factory);

        // Assert
        evento.Properties[CorrelationConstants.LogPropertyName].ToString().Trim('"').ShouldBe("valor-existente");
    }

    [Fact(DisplayName = "Não deve adicionar propriedade quando contexto não possui correlation id")]
    [Trait("Infrastructure", "CorrelationIdEnricher")]
    public void Enrich_NaoDeveAdicionarPropriedade_QuandoContextoNaoPossuirCorrelationId()
    {
        // Arrange
        var enricher = new CorrelationIdEnricher();
        var evento = CriarEventoLog();
        var factory = new PropertyFactoryStub();

        // Act
        enricher.Enrich(evento, factory);

        // Assert
        evento.Properties.ContainsKey(CorrelationConstants.LogPropertyName).ShouldBeFalse();
    }

    private static LogEvent CriarEventoLog()
    {
        return new LogEvent(DateTimeOffset.UtcNow, LogEventLevel.Information, null, new MessageTemplate("mensagem", []), []);
    }

    private sealed class PropertyFactoryStub : ILogEventPropertyFactory
    {
        public LogEventProperty CreateProperty(string name, object? value, bool destructureObjects = false)
        {
            return new LogEventProperty(name, new ScalarValue(value));
        }
    }
}
