namespace Application.Contracts.Monitoramento;

public interface IMetricsService
{
    void RegistrarProcessamentoIniciado(Guid analiseDiagramaId);
    void RegistrarProcessamentoConcluido(Guid analiseDiagramaId, long duracaoMs);
    void RegistrarProcessamentoFalha(Guid analiseDiagramaId, string motivo, int tentativasRealizadas);
    void RegistrarProcessamentoRejeitado(Guid analiseDiagramaId, string motivo, int tentativasRealizadas);
}
