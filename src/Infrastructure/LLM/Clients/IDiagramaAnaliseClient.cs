using Application.Contracts.LLM;

namespace Infrastructure.LLM;

public interface IDiagramaAnaliseClient
{
    Task<ResultadoAnaliseDto> AnalisarDiagramaAsync(Guid analiseDiagramaId, string nomeFisico, byte[] conteudoArquivo, string extensao);
}