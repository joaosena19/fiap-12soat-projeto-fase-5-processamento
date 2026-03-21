using Infrastructure.Monitoramento.Correlation;
using MassTransit;

namespace Infrastructure.Messaging.Filters;

/// <summary>
/// Filtro MassTransit que estabelece correlation scope para todos os consumers.
/// </summary>
public class ConsumeCorrelationIdFilter<T> : IFilter<ConsumeContext<T>> where T : class
{
    public void Probe(ProbeContext context)
    {
        context.CreateFilterScope("correlationId");
    }

    public async Task Send(ConsumeContext<T> context, IPipe<ConsumeContext<T>> next)
    {
        var correlationId = ExtractCorrelationId(context);

        using (CorrelationContext.Push(correlationId))
        using (Serilog.Context.LogContext.PushProperty(CorrelationConstants.LogPropertyName, correlationId))
            await next.Send(context);
    }

    private static string ExtractCorrelationId(ConsumeContext<T> context)
    {
        if (context.Headers.TryGetHeader(CorrelationConstants.HeaderName, out var headerValue) && headerValue is string headerString && !string.IsNullOrWhiteSpace(headerString))
            return headerString;

        if (context.CorrelationId.HasValue)
            return context.CorrelationId.Value.ToString();

        var messageType = context.Message.GetType();
        var correlationIdProperty = messageType.GetProperty("CorrelationId");
        if (correlationIdProperty != null)
        {
            var value = correlationIdProperty.GetValue(context.Message);
            if (value != null)
            {
                var valueStr = value.ToString();
                if (!string.IsNullOrWhiteSpace(valueStr))
                    return valueStr;
            }
        }

        return Guid.NewGuid().ToString();
    }
}
