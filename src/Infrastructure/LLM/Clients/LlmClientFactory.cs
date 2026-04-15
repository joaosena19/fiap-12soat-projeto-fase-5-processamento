using Application.Contracts.LLM;
using GenerativeAI.Microsoft;
using Microsoft.Extensions.Logging;

namespace Infrastructure.LLM;

public sealed class LlmClientFactory : ILlmClientFactory
{
    private readonly LlmOptions _opcoes;
    private readonly ILoggerFactory _loggerFactory;

    public LlmClientFactory(LlmOptions opcoes, ILoggerFactory loggerFactory)
    {
        _opcoes = opcoes;
        _loggerFactory = loggerFactory;
    }

    public IDiagramaAnaliseClient CriarPara(string modelo) => new LlmDiagramaAnaliseClient(new GenerativeAIChatClient(_opcoes.ApiKey, modelo), _loggerFactory, modelo);
}
