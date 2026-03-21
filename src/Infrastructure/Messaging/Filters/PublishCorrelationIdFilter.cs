using Infrastructure.Monitoramento.Correlation;
using MassTransit;

namespace Infrastructure.Messaging.Filters;

/// <summary>
/// Filtro MassTransit que propaga correlation ID automaticamente em todas as mensagens publicadas.
/// </summary>
public class PublishCorrelationIdFilter<T> : IFilter<PublishContext<T>> where T : class
{
    public void Probe(ProbeContext context)
    {
        context.CreateFilterScope("correlationId");
    }

    public async Task Send(PublishContext<T> context, IPipe<PublishContext<T>> next)
    {
        var correlationId = CorrelationContext.Current ?? Guid.NewGuid().ToString();

        context.Headers.Set(CorrelationConstants.HeaderName, correlationId);

        if (Guid.TryParse(correlationId, out var guid))
            context.CorrelationId = guid;

        await next.Send(context);
    }
}
