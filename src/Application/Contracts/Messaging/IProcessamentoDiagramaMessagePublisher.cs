namespace Application.Contracts.Messaging;

public interface IProcessamentoDiagramaMessagePublisher
{
    Task PublicarProcessamentoIniciadoAsync(Domain.ProcessamentoDiagrama.Aggregates.ProcessamentoDiagrama processamentoDiagrama, string nomeOriginal, string extensao);
    Task PublicarDiagramaAnalisadoAsync(Domain.ProcessamentoDiagrama.Aggregates.ProcessamentoDiagrama processamentoDiagrama);
    Task PublicarProcessamentoErroAsync(Domain.ProcessamentoDiagrama.Aggregates.ProcessamentoDiagrama processamentoDiagrama, string motivo, string? origemErro = null, bool rejeitado = false, bool podeRetentar = true);
}
