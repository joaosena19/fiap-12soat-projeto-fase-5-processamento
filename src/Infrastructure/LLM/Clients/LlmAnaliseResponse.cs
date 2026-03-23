namespace Infrastructure.LLM;

public sealed class LlmAnaliseResponse
{
    public string DescricaoAnalise { get; set; } = string.Empty;
    public List<string> ComponentesIdentificados { get; set; } = [];
    public List<string> RiscosArquiteturais { get; set; } = [];
    public List<string> RecomendacoesBasicas { get; set; } = [];
}