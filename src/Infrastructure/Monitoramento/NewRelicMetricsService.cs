using Application.Contracts.Monitoramento;
using Shared.Constants;
using Infrastructure.Monitoramento.Correlation;
using NR = NewRelic.Api.Agent;

namespace Infrastructure.Monitoramento;

/// <summary>
/// Implementação de métricas customizadas usando New Relic.
/// </summary>
public class NewRelicMetricsService : IMetricsService
{
    public void RegistrarProcessamentoIniciado(Guid analiseDiagramaId)
    {
        var atributos = new Dictionary<string, object>
        {
            { LogNomesPropriedades.AnaliseDiagramaId, analiseDiagramaId },
            { LogNomesPropriedades.Timestamp, DateTimeOffset.UtcNow }
        };

        AdicionarCorrelationId(atributos);

        NR.NewRelic.RecordCustomEvent("ProcessamentoDiagramaIniciado", atributos);
    }

    public void RegistrarProcessamentoConcluido(Guid analiseDiagramaId, long duracaoMs)
    {
        var atributos = new Dictionary<string, object>
        {
            { LogNomesPropriedades.AnaliseDiagramaId, analiseDiagramaId },
            { LogNomesPropriedades.DuracaoMs, duracaoMs },
            { LogNomesPropriedades.Timestamp, DateTimeOffset.UtcNow }
        };

        AdicionarCorrelationId(atributos);

        NR.NewRelic.RecordCustomEvent("ProcessamentoDiagramaConcluido", atributos);
    }

    public void RegistrarProcessamentoFalha(Guid analiseDiagramaId, string motivo, int tentativasRealizadas)
    {
        var atributos = new Dictionary<string, object>
        {
            { LogNomesPropriedades.AnaliseDiagramaId, analiseDiagramaId },
            { LogNomesPropriedades.Motivo, motivo },
            { LogNomesPropriedades.Tentativas, tentativasRealizadas },
            { LogNomesPropriedades.Timestamp, DateTimeOffset.UtcNow }
        };

        AdicionarCorrelationId(atributos);

        NR.NewRelic.RecordCustomEvent("ProcessamentoDiagramaFalha", atributos);
    }

    private static void AdicionarCorrelationId(Dictionary<string, object> atributos)
    {
        var correlationId = CorrelationContext.Current;
        if (!string.IsNullOrWhiteSpace(correlationId))
            atributos[LogNomesPropriedades.CorrelationId] = correlationId;
    }
}
