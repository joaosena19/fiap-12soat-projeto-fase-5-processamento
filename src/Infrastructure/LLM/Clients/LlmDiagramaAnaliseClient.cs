using System.Text.Json;
using Application.Contracts.LLM;
using Application.Contracts.Monitoramento;
using Infrastructure.Monitoramento;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using Shared.Constants;

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
    private readonly string _modelo;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public LlmDiagramaAnaliseClient(IChatClient chatClient, ILoggerFactory loggerFactory, string modelo)
    {
        _chatClient = chatClient;
        _logger = loggerFactory.CriarAppLogger<LlmDiagramaAnaliseClient>();
        _modelo = modelo;
    }

    public async Task<ResultadoAnaliseDto> AnalisarDiagramaAsync(Guid analiseDiagramaId, string nomeFisico, byte[] conteudoArquivo, string extensao)
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

        try
        {
            _logger.ComPropriedade(LogNomesPropriedades.AnaliseDiagramaId, analiseDiagramaId).LogDebug("Iniciando chamada à LLM ({Modelo}) para {AnaliseDiagramaId}. Arquivo: {NomeFisico}, MediaType: {MediaType}", _modelo, analiseDiagramaId, nomeFisico, mediaType);

            var resposta = await _chatClient.GetResponseAsync(mensagens, opcoes);
            var textoResposta = resposta.Text;

            if (textoResposta == null)
            {
                _logger.ComPropriedade(LogNomesPropriedades.AnaliseDiagramaId, analiseDiagramaId).LogWarning("A LLM retornou uma resposta nula para {AnaliseDiagramaId}. FinishReason: {FinishReason}", analiseDiagramaId, resposta.FinishReason?.ToString() ?? "null");
                throw new LlmTransientException("A LLM retornou uma resposta nula.");
            }

            _logger.ComPropriedade(LogNomesPropriedades.AnaliseDiagramaId, analiseDiagramaId).LogDebug("Resposta recebida da LLM para {AnaliseDiagramaId}. Tamanho: {TamanhoResposta} chars", analiseDiagramaId, textoResposta.Length);

            var analise = JsonSerializer.Deserialize<LlmAnaliseResponse>(textoResposta, JsonOptions);

            if (analise == null)
            {
                _logger.ComPropriedade(LogNomesPropriedades.AnaliseDiagramaId, analiseDiagramaId).LogWarning("Falha ao desserializar a resposta da LLM para {AnaliseDiagramaId}. Resposta bruta: {RespostaBruta}", analiseDiagramaId, textoResposta.Length > 500 ? textoResposta[..500] + "..." : textoResposta);
                throw new LlmTransientException("Falha ao desserializar a resposta da LLM.");
            }

            return MapearResultadoAnalise(analise);
        }
        catch (LlmPermanentException)
        {
            throw;
        }
        catch (LlmTransientException)
        {
            throw;
        }
        catch (Exception ex)
        {
            var codigoHttp = ExtrairCodigoHttp(ex);

            if (codigoHttp is 429 or 503)
            {
                _logger.ComPropriedade(LogNomesPropriedades.AnaliseDiagramaId, analiseDiagramaId)
                       .LogWarning(ex, "Modelo {Modelo} retornou HTTP {CodigoHttp} para {AnaliseDiagramaId}.", _modelo, codigoHttp, analiseDiagramaId);
                throw new LlmIndisponivelException(_modelo, codigoHttp, $"Modelo {_modelo} indisponível (HTTP {codigoHttp}): {ex.Message}", ex);
            }

            _logger.ComPropriedade(LogNomesPropriedades.AnaliseDiagramaId, analiseDiagramaId)
                   .LogError(ex, "Erro na chamada à LLM para {AnaliseDiagramaId}. Modelo: {Modelo}, ExceptionType: {ExceptionType}", analiseDiagramaId, _modelo, ex.GetType().FullName ?? ex.GetType().Name);
            throw new LlmTransientException($"Falha transitória ao consultar a LLM: {ex.Message}", ex);
        }
    }

    private static ResultadoAnaliseDto MapearResultadoAnalise(LlmAnaliseResponse analise)
    {
        if (!analise.EhDiagramaArquitetural)
        {
            if (string.IsNullOrWhiteSpace(analise.MotivoInvalidez))
                throw new LlmPermanentException("A LLM retornou EhDiagramaArquitetural=false sem MotivoInvalidez.");

            return new ResultadoAnaliseDto
            {
                Sucesso = false,
                MotivoErro = analise.MotivoInvalidez,
                TentativasRealizadas = 1,
                OrigemErro = OrigemErroConstantes.LlmValidacao,
                Rejeitado = true,
                PodeRetentar = false
            };
        }

        if (string.IsNullOrWhiteSpace(analise.DescricaoAnalise))
            throw new LlmPermanentException("A LLM retornou EhDiagramaArquitetural=true sem DescricaoAnalise.");

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

    private static int? ExtrairCodigoHttp(Exception ex)
    {
        var atual = ex;
        while (atual != null)
        {
            if (atual is HttpRequestException httpEx && httpEx.StatusCode.HasValue)
                return (int)httpEx.StatusCode.Value;

            atual = atual.InnerException;
        }

        if (ex.Message.Contains("429") || ex.Message.Contains("Too Many Requests"))
            return 429;
        if (ex.Message.Contains("503") || ex.Message.Contains("Service Unavailable"))
            return 503;

        return null;
    }
}