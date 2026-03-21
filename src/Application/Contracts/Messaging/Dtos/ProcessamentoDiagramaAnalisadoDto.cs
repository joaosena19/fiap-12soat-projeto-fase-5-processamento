namespace Application.Contracts.Messaging.Dtos;

public record ProcessamentoDiagramaAnalisadoDto
{
    public string CorrelationId { get; init; } = string.Empty;
    public Guid AnaliseDiagramaId { get; init; }
    public string DescricaoAnalise { get; init; } = string.Empty;
    public List<string> ComponentesIdentificados { get; init; } = new();
    public List<string> RiscosArquiteturais { get; init; } = new();
    public List<string> RecomendacoesBasicas { get; init; } = new();
    public DateTimeOffset DataConclusao { get; init; }
}