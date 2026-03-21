namespace Application.Contracts.Messaging.Dtos;

public record ProcessamentoDiagramaErroDto
{
    public string CorrelationId { get; init; } = string.Empty;
    public Guid AnaliseDiagramaId { get; init; }
    public string Motivo { get; init; } = string.Empty;
    public int TentativasRealizadas { get; init; }
    public DateTimeOffset DataErro { get; init; }
}