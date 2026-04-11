namespace Infrastructure.Armazenamento;

public interface IArquivoDiagramaDownloader
{
    Task<byte[]> BaixarArquivoAsync(string localizacaoUrl);
}