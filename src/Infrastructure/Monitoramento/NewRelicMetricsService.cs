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
    private const string EventoProcessamentoIniciado = "ProcessamentoDiagramaIniciado";
    private const string EventoProcessamentoConcluido = "ProcessamentoDiagramaConcluido";
    private const string EventoProcessamentoFalha = "ProcessamentoDiagramaFalha";

    public void RegistrarProcessamentoIniciado(Guid analiseDiagramaId)
    {
        RegistrarEvento(EventoProcessamentoIniciado, new Dictionary<string, object>
        {
            { LogNomesPropriedades.AnaliseDiagramaId, analiseDiagramaId }
        });
    }

    public void RegistrarProcessamentoConcluido(Guid analiseDiagramaId, long duracaoMs)
    {
        RegistrarEvento(EventoProcessamentoConcluido, new Dictionary<string, object>
        {
            { LogNomesPropriedades.AnaliseDiagramaId, analiseDiagramaId },
            { LogNomesPropriedades.DuracaoMs, duracaoMs }
        });
    }

    public void RegistrarProcessamentoFalha(Guid analiseDiagramaId, string motivo, int tentativasRealizadas)
    {
        RegistrarEvento(EventoProcessamentoFalha, new Dictionary<string, object>
        {
            { LogNomesPropriedades.AnaliseDiagramaId, analiseDiagramaId },
            { LogNomesPropriedades.Motivo, motivo },
            { LogNomesPropriedades.Tentativas, tentativasRealizadas }
        });
    }

    private static void RegistrarEvento(string nomeEvento, Dictionary<string, object> atributos)
    {
        atributos[LogNomesPropriedades.Timestamp] = DateTimeOffset.UtcNow;
        AdicionarCorrelationId(atributos);
        NR.NewRelic.RecordCustomEvent(nomeEvento, atributos);
    }

    private static void AdicionarCorrelationId(Dictionary<string, object> atributos)
    {
        var correlationId = CorrelationContext.Current;
        if (!string.IsNullOrWhiteSpace(correlationId))
            atributos[LogNomesPropriedades.CorrelationId] = correlationId;
    }
}
