namespace Application.Contracts.LLM;

public interface IDiagramaAnaliseService
{
    Task<ResultadoAnaliseDto> AnalisarDiagramaAsync(string nomeFisico, string localizacaoUrl, string extensao);
}
