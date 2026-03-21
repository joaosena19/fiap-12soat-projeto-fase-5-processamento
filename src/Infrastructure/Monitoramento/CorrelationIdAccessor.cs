using Application.Contracts.Monitoramento;
using Infrastructure.Monitoramento.Correlation;

namespace Infrastructure.Monitoramento;

/// <summary>
/// Implementação do ICorrelationIdAccessor que lê do CorrelationContext.
/// </summary>
public class CorrelationIdAccessor : ICorrelationIdAccessor
{
    public string GetCorrelationId()
    {
        var current = CorrelationContext.Current;
        if (!string.IsNullOrWhiteSpace(current))
            return current;

        return Guid.NewGuid().ToString();
    }
}
