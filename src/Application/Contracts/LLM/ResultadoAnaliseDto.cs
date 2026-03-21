namespace Application.Contracts.LLM;

public record ResultadoAnaliseDto
{
    public bool Sucesso { get; init; }
    public string? DescricaoAnalise { get; init; }
    public List<string> ComponentesIdentificados { get; init; } = new();
    public List<string> RiscosArquiteturais { get; init; } = new();
    public List<string> RecomendacoesBasicas { get; init; } = new();
    public string? MotivoErro { get; init; }
    public int TentativasRealizadas { get; init; }
}