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
    private readonly IAppLogger _logger;

    public DiagramaAnaliseService(IDiagramaAnaliseClient client, ILoggerFactory loggerFactory)
    {
        _client = client;
        _logger = new LoggerAdapter<DiagramaAnaliseService>(loggerFactory.CreateLogger<DiagramaAnaliseService>());
    }

    public async Task<ResultadoAnaliseDto> AnalisarDiagramaAsync(Guid analiseDiagramaId, string nomeFisico, string localizacaoUrl, string extensao)
    {
        var tentativasRealizadas = 0;
        var cronometro = Stopwatch.StartNew();

        var pipeline = new ResiliencePipelineBuilder<ResultadoAnaliseDto>()
            .AddRetry(new RetryStrategyOptions<ResultadoAnaliseDto>
            {
                MaxRetryAttempts = 4,
                BackoffType = DelayBackoffType.Exponential,
                Delay = TimeSpan.FromSeconds(1),
                UseJitter = true,
                ShouldHandle = new PredicateBuilder<ResultadoAnaliseDto>()
                    .Handle<Exception>()
                    .HandleResult(resultado => resultado == null || !resultado.Sucesso),
                OnRetry = args =>
                {
                    tentativasRealizadas = args.AttemptNumber + 1;
                    _logger.LogWarning($"Nova tentativa de análise do diagrama para {{{LogNomesPropriedades.AnaliseDiagramaId}}}. {{{LogNomesPropriedades.Tentativas}}}", analiseDiagramaId, tentativasRealizadas + 1);
                    return default;
                }
            })
            .Build();

        try
        {
            _logger.LogDebug($"Iniciando chamada de análise da LLM para {{{LogNomesPropriedades.AnaliseDiagramaId}}}", analiseDiagramaId);

            var resultado = await pipeline.ExecuteAsync(async cancellationToken =>
            {
                tentativasRealizadas++;
                return await _client.AnalisarDiagramaAsync(nomeFisico, localizacaoUrl, extensao);
            }, CancellationToken.None);

            _logger.LogDebug($"Chamada de análise da LLM concluída para {{{LogNomesPropriedades.AnaliseDiagramaId}}} em {{{LogNomesPropriedades.DuracaoMs}}}ms", analiseDiagramaId, cronometro.ElapsedMilliseconds);

            return resultado with { TentativasRealizadas = tentativasRealizadas };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Falha ao analisar diagrama na LLM para {{{LogNomesPropriedades.AnaliseDiagramaId}}} após {{{LogNomesPropriedades.Tentativas}}} tentativa(s)", analiseDiagramaId, Math.Max(tentativasRealizadas, 5));

            return new ResultadoAnaliseDto
            {
                Sucesso = false,
                MotivoErro = ex.Message,
                TentativasRealizadas = Math.Max(tentativasRealizadas, 5)
            };
        }
    }
}