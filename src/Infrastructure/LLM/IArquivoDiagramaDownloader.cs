namespace Infrastructure.LLM;

public interface IArquivoDiagramaDownloader
{
    Task<byte[]> BaixarArquivoAsync(string localizacaoUrl);
}