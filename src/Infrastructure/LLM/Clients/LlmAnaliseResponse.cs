namespace Infrastructure.LLM;

public sealed class LlmAnaliseResponse
{
    public bool EhDiagramaArquitetural { get; set; }
    public string? MotivoInvalidez { get; set; }
    public string? DescricaoAnalise { get; set; }
    public List<string> ComponentesIdentificados { get; set; } = [];
    public List<string> RiscosArquiteturais { get; set; } = [];
    public List<string> RecomendacoesBasicas { get; set; } = [];
}