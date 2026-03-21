using Application.Contracts.LLM;

namespace Infrastructure.LLM;

public interface IDiagramaAnaliseClient
{
    Task<ResultadoAnaliseDto> AnalisarDiagramaAsync(string nomeFisico, string localizacaoUrl, string extensao);
}