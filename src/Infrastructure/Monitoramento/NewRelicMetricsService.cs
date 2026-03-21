using Application.Contracts.Monitoramento;
using Shared.Constants;
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
        NR.NewRelic.RecordCustomEvent("ProcessamentoDiagramaConcluido", atributos);
    }

    public void RegistrarProcessamentoFalha(Guid analiseDiagramaId, string motivo)
    {
        var atributos = new Dictionary<string, object>
        {
            { LogNomesPropriedades.AnaliseDiagramaId, analiseDiagramaId },
            { LogNomesPropriedades.Motivo, motivo },
            { LogNomesPropriedades.Timestamp, DateTimeOffset.UtcNow }
        };
        NR.NewRelic.RecordCustomEvent("ProcessamentoDiagramaFalha", atributos);
    }
}
