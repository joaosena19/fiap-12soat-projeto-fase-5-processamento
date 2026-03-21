namespace Application.Contracts.Messaging.Dtos;

public record ProcessamentoDiagramaIniciadoDto
{
    public string CorrelationId { get; init; } = string.Empty;
    public Guid AnaliseDiagramaId { get; init; }
    public string NomeOriginal { get; init; } = string.Empty;
    public string Extensao { get; init; } = string.Empty;
    public DateTimeOffset DataInicio { get; init; }
}