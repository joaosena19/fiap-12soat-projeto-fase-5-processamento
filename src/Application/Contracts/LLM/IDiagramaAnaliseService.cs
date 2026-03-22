namespace Application.Contracts.LLM;

public interface IDiagramaAnaliseService
{
    Task<ResultadoAnaliseDto> AnalisarDiagramaAsync(Guid analiseDiagramaId, string nomeFisico, string localizacaoUrl, string extensao);
}
