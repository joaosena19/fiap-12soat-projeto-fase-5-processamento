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
    private const string MediaTypePng = "image/png";
    private const string MediaTypeJpeg = "image/jpeg";
    private const string MediaTypeGif = "image/gif";
    private const string MediaTypeWebp = "image/webp";
    private const string MediaTypePdf = "application/pdf";
    private const string MediaTypePadrao = "application/octet-stream";

    private readonly IChatClient _chatClient;
    private readonly IAppLogger _logger;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public LlmDiagramaAnaliseClient(IChatClient chatClient, ILoggerFactory loggerFactory)
    {
        _chatClient = chatClient;
        _logger = loggerFactory.CriarAppLogger<LlmDiagramaAnaliseClient>();
    }

    public async Task<ResultadoAnaliseDto> AnalisarDiagramaAsync(string nomeFisico, byte[] conteudoArquivo, string extensao)
    {
        var mediaType = ObterMediaType(extensao);

        var mensagens = new List<ChatMessage>
        {
            new(ChatRole.System, DiagramaAnalisePrompts.SystemPrompt),
            new(ChatRole.User, new List<AIContent>
            {
                new TextContent(DiagramaAnalisePrompts.UserPrompt),
                new DataContent(conteudoArquivo, mediaType)
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

        var analise = JsonSerializer.Deserialize<LlmAnaliseResponse>(textoResposta, JsonOptions);

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
        ".png" => MediaTypePng,
        ".jpg" or ".jpeg" => MediaTypeJpeg,
        ".gif" => MediaTypeGif,
        ".webp" => MediaTypeWebp,
        ".pdf" => MediaTypePdf,
        _ => MediaTypePadrao
    };
}