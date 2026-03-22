namespace Infrastructure.LLM;

public record LlmAnaliseResponse
{
    public string DescricaoAnalise { get; init; } = string.Empty;
    public List<string> ComponentesIdentificados { get; init; } = new();
    public List<string> RiscosArquiteturais { get; init; } = new();
    public List<string> RecomendacoesBasicas { get; init; } = new();
}
