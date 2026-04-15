using Application.Contracts.LLM;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;

namespace Infrastructure.LLM;

internal static class ResilienciaAnaliseDiagramaPipelineFactory
{
    public static ResiliencePipeline<ResultadoAnaliseDto> Criar(ResilienciaAnaliseDiagramaOptions options, ILogger? logger = null)
    {
        return new ResiliencePipelineBuilder<ResultadoAnaliseDto>()
            .AddRetry(new RetryStrategyOptions<ResultadoAnaliseDto>
            {
                MaxRetryAttempts = options.MaxTentativasPorModelo,
                BackoffType = DelayBackoffType.Exponential,
                Delay = TimeSpan.FromSeconds(options.DelaySegundos),
                UseJitter = true,
                ShouldHandle = new PredicateBuilder<ResultadoAnaliseDto>()
                    .Handle<LlmTransientException>(ex => ex is not LlmIndisponivelException)
                    .HandleResult(resultado => resultado == null),
                OnRetry = args =>
                {
                    if (logger != null && args.Outcome.Exception != null)
                        logger.LogWarning(args.Outcome.Exception, "Retry {AttemptNumber}/{MaxRetry} na chamada LLM. Motivo: {RetryMotivo}. Próximo delay: {RetryDelay:F1}s", args.AttemptNumber + 1, options.MaxTentativasPorModelo, args.Outcome.Exception.Message, args.RetryDelay.TotalSeconds);
                    else if (logger != null)
                        logger.LogWarning("Retry {AttemptNumber}/{MaxRetry} na chamada LLM. Resultado nulo. Próximo delay: {RetryDelay:F1}s", args.AttemptNumber + 1, options.MaxTentativasPorModelo, args.RetryDelay.TotalSeconds);

                    return default;
                }
            })
            .Build();
    }
}