using Application.Contracts.LLM;
using Application.Contracts.Monitoramento;
using Infrastructure.Monitoramento;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using Shared.Constants;
using System.Diagnostics;

namespace Infrastructure.LLM;

public class DiagramaAnaliseService : IDiagramaAnaliseService
{
    private readonly IDiagramaAnaliseClient _client;
    private readonly IArquivoDiagramaDownloader _arquivoDiagramaDownloader;
    private readonly IAppLogger _logger;
    private readonly ResiliencePipeline<ResultadoAnaliseDto> _pipeline;

    public DiagramaAnaliseService(IDiagramaAnaliseClient client, IArquivoDiagramaDownloader arquivoDiagramaDownloader, ILoggerFactory loggerFactory)
    {
        _client = client;
        _arquivoDiagramaDownloader = arquivoDiagramaDownloader;
        _logger = new LoggerAdapter<DiagramaAnaliseService>(loggerFactory.CreateLogger<DiagramaAnaliseService>());
        _pipeline = new ResiliencePipelineBuilder<ResultadoAnaliseDto>()
            .AddRetry(new RetryStrategyOptions<ResultadoAnaliseDto>
            {
                MaxRetryAttempts = 4,
                BackoffType = DelayBackoffType.Exponential,
                Delay = TimeSpan.FromSeconds(1),
                UseJitter = true,
                ShouldHandle = new PredicateBuilder<ResultadoAnaliseDto>()
                    .Handle<Exception>()
                    .HandleResult(resultado => resultado == null || !resultado.Sucesso)
            })
            .Build();
    }

    public async Task<ResultadoAnaliseDto> AnalisarDiagramaAsync(Guid analiseDiagramaId, string nomeFisico, string localizacaoUrl, string extensao)
    {
        var tentativasRealizadas = 0;
        var cronometro = Stopwatch.StartNew();

        try
        {
            _logger.LogDebug($"Iniciando chamada de análise da LLM para {{{LogNomesPropriedades.AnaliseDiagramaId}}}", analiseDiagramaId);
            var conteudoArquivo = await _arquivoDiagramaDownloader.BaixarArquivoAsync(localizacaoUrl);

            var resultado = await _pipeline.ExecuteAsync(async cancellationToken =>
            {
                tentativasRealizadas++;
                if (tentativasRealizadas > 1)
                    _logger.LogWarning($"Nova tentativa de análise do diagrama para {{{LogNomesPropriedades.AnaliseDiagramaId}}}. {{{LogNomesPropriedades.Tentativas}}}", analiseDiagramaId, tentativasRealizadas);
                return await _client.AnalisarDiagramaAsync(nomeFisico, conteudoArquivo, extensao);
            }, CancellationToken.None);

            _logger.LogDebug($"Chamada de análise da LLM concluída para {{{LogNomesPropriedades.AnaliseDiagramaId}}} em {{{LogNomesPropriedades.DuracaoMs}}}ms", analiseDiagramaId, cronometro.ElapsedMilliseconds);

            return resultado with { TentativasRealizadas = tentativasRealizadas };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Falha ao analisar diagrama na LLM para {{{LogNomesPropriedades.AnaliseDiagramaId}}} após {{{LogNomesPropriedades.Tentativas}}} tentativa(s)", analiseDiagramaId, tentativasRealizadas);

            return new ResultadoAnaliseDto
            {
                Sucesso = false,
                MotivoErro = ex.Message,
                TentativasRealizadas = tentativasRealizadas
            };
        }
    }
}