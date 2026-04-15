using Application.Contracts.LLM;

namespace Infrastructure.LLM;

public interface ILlmClientFactory
{
    IDiagramaAnaliseClient CriarPara(string modelo);
}
