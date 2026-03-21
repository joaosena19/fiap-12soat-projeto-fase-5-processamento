namespace Application.Contracts.Gateways;

public interface IProcessamentoDiagramaGateway
{
    Task<Domain.ProcessamentoDiagrama.Aggregates.ProcessamentoDiagrama> SalvarAsync(Domain.ProcessamentoDiagrama.Aggregates.ProcessamentoDiagrama processamentoDiagrama);
    Task<Domain.ProcessamentoDiagrama.Aggregates.ProcessamentoDiagrama?> ObterPorAnaliseDiagramaIdAsync(Guid analiseDiagramaId);
}
