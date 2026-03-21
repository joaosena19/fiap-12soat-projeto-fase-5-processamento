using Serilog.Core;
using Serilog.Events;

namespace Infrastructure.Monitoramento.Correlation;

/// <summary>
/// Enriquecedor Serilog que adiciona o correlation ID aos eventos de log.
/// </summary>
public class CorrelationIdEnricher : ILogEventEnricher
{
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        if (logEvent.Properties.ContainsKey(CorrelationConstants.LogPropertyName))
            return;

        var correlationId = CorrelationContext.Current;

        if (!string.IsNullOrEmpty(correlationId))
        {
            var property = new LogEventProperty(CorrelationConstants.LogPropertyName, new ScalarValue(correlationId));
            logEvent.AddPropertyIfAbsent(property);
        }
    }
}
