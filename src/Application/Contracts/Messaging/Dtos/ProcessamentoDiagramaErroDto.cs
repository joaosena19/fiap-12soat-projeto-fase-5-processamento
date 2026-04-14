namespace Application.Contracts.Messaging.Dtos;

public record ProcessamentoDiagramaErroDto
{
    public string CorrelationId { get; init; } = string.Empty;
    public Guid AnaliseDiagramaId { get; init; }
    public string Motivo { get; init; } = string.Empty;
    public string? OrigemErro { get; init; }
    public int TentativasRealizadas { get; init; }
    public bool Rejeitado { get; init; }
    public bool PodeRetentar { get; init; } = true;
    public DateTimeOffset DataErro { get; init; }
}