using Infrastructure.Monitoramento.Correlation;
using MassTransit;

namespace Infrastructure.Messaging.Filters;

/// <summary>
/// Filtro MassTransit que propaga correlation ID automaticamente em todas as mensagens enviadas (Send).
/// </summary>
public class SendCorrelationIdFilter<T> : IFilter<SendContext<T>> where T : class
{
    public void Probe(ProbeContext context)
    {
        context.CreateFilterScope("correlationId");
    }

    public async Task Send(SendContext<T> context, IPipe<SendContext<T>> next)
    {
        var correlationId = CorrelationContext.Current ?? Guid.NewGuid().ToString();

        context.Headers.Set(CorrelationConstants.HeaderName, correlationId);

        if (Guid.TryParse(correlationId, out var guid))
            context.CorrelationId = guid;

        await next.Send(context);
    }
}
