using System.Text.Json;
using Application.Contracts.LLM;
using Application.Contracts.Monitoramento;
using Infrastructure.Monitoramento;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;

namespace Infrastructure.LLM;

/// <summary>
/// Client de análise de diagramas via LLM usando Microsoft.Extensions.AI.
/// </summary>
public class LlmDiagramaAnaliseClient : IDiagramaAnaliseClient
{
    private readonly IChatClient _chatClient;
    private readonly S3ArquivoDownloader _downloader;
    private readonly IAppLogger _logger;

    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public LlmDiagramaAnaliseClient(IChatClient chatClient, S3ArquivoDownloader downloader, ILoggerFactory loggerFactory)
    {
        _chatClient = chatClient;
        _downloader = downloader;
        _logger = new LoggerAdapter<LlmDiagramaAnaliseClient>(loggerFactory.CreateLogger<LlmDiagramaAnaliseClient>());
    }

    public async Task<ResultadoAnaliseDto> AnalisarDiagramaAsync(string nomeFisico, string localizacaoUrl, string extensao)
    {
        var bytesArquivo = await _downloader.BaixarArquivoAsync(localizacaoUrl);
        var mediaType = ObterMediaType(extensao);

        var mensagens = new List<ChatMessage>
        {
            new(ChatRole.System, DiagramaAnalisePrompts.SystemPrompt),
            new(ChatRole.User, new List<AIContent>
            {
                new TextContent(DiagramaAnalisePrompts.UserPrompt),
                new DataContent(bytesArquivo, mediaType)
            })
        };

        var opcoes = new ChatOptions
        {
            Temperature = 0.1f,
            ResponseFormat = ChatResponseFormat.ForJsonSchema<LlmAnaliseResponse>()
        };

        var resposta = await _chatClient.GetResponseAsync(mensagens, opcoes);

        var textoResposta = resposta.Text;

        if (textoResposta == null)
        {
            _logger.LogWarning("A LLM retornou uma resposta nula");
            throw new InvalidOperationException("A LLM retornou uma resposta nula.");
        }

        var analise = JsonSerializer.Deserialize<LlmAnaliseResponse>(textoResposta, _jsonOptions);

        if (analise == null)
        {
            _logger.LogWarning("Falha ao desserializar a resposta da LLM");
            throw new InvalidOperationException("Falha ao desserializar a resposta da LLM.");
        }

        return new ResultadoAnaliseDto
        {
            Sucesso = true,
            DescricaoAnalise = analise.DescricaoAnalise,
            ComponentesIdentificados = analise.ComponentesIdentificados,
            RiscosArquiteturais = analise.RiscosArquiteturais,
            RecomendacoesBasicas = analise.RecomendacoesBasicas,
            TentativasRealizadas = 1
        };
    }

    private static string ObterMediaType(string extensao) => extensao.ToLowerInvariant() switch
    {
        ".png" => "image/png",
        ".jpg" or ".jpeg" => "image/jpeg",
        ".gif" => "image/gif",
        ".webp" => "image/webp",
        ".pdf" => "application/pdf",
        _ => "application/octet-stream"
    };
}
