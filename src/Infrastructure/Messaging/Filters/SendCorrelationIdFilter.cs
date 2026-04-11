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
        CorrelationIdFilterHelper.AplicarCorrelationId(context);
        await next.Send(context);
    }
}
