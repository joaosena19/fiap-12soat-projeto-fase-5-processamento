namespace Application.Contracts.Monitoramento;

public interface ICorrelationIdAccessor
{
    string GetCorrelationId();
}
