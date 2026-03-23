using Application.Contracts.LLM;
using Polly;
using Polly.Retry;

namespace Infrastructure.LLM;

internal static class ResilienciaAnaliseDiagramaPipelineFactory
{
    public static ResiliencePipeline<ResultadoAnaliseDto> Criar(ResilienciaAnaliseDiagramaOptions options)
    {
        return new ResiliencePipelineBuilder<ResultadoAnaliseDto>()
            .AddRetry(new RetryStrategyOptions<ResultadoAnaliseDto>
            {
                MaxRetryAttempts = options.MaxTentativas,
                BackoffType = DelayBackoffType.Exponential,
                Delay = TimeSpan.FromSeconds(options.DelaySegundos),
                UseJitter = true,
                ShouldHandle = new PredicateBuilder<ResultadoAnaliseDto>()
                    .Handle<Exception>()
                    .HandleResult(resultado => resultado == null || !resultado.Sucesso)
            })
            .Build();
    }
}