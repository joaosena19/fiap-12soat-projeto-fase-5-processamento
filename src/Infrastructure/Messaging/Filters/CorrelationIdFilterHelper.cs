using Infrastructure.Monitoramento.Correlation;
using MassTransit;

namespace Infrastructure.Messaging.Filters;

/// <summary>
/// Lógica compartilhada entre filtros de envio (Send) e publicação (Publish) para propagação de correlation ID.
/// </summary>
public static class CorrelationIdFilterHelper
{
    public static void AplicarCorrelationId<T>(SendContext<T> context) where T : class
    {
        var correlationId = CorrelationContext.Current ?? Guid.NewGuid().ToString();

        context.Headers.Set(CorrelationConstants.HeaderName, correlationId);

        if (Guid.TryParse(correlationId, out var guid))
            context.CorrelationId = guid;
    }
}
