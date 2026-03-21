namespace Application.Contracts.Messaging.Dtos;

public record UploadDiagramaConcluidoDto
{
    public string CorrelationId { get; init; } = string.Empty;
    public Guid AnaliseDiagramaId { get; init; }
    public string NomeOriginal { get; init; } = string.Empty;
    public string Extensao { get; init; } = string.Empty;
    public long Tamanho { get; init; }
    public string Hash { get; init; } = string.Empty;
    public string NomeFisico { get; init; } = string.Empty;
    public string LocalizacaoUrl { get; init; } = string.Empty;
    public DateTimeOffset DataCriacao { get; init; }
}