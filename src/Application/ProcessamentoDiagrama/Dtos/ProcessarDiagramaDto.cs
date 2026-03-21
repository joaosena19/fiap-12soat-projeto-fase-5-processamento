namespace Application.ProcessamentoDiagrama.Dtos;

public record ProcessarDiagramaDto
{
    public Guid AnaliseDiagramaId { get; init; }
    public string NomeOriginal { get; init; } = string.Empty;
    public string Extensao { get; init; } = string.Empty;
    public string NomeFisico { get; init; } = string.Empty;
    public string LocalizacaoUrl { get; init; } = string.Empty;
}