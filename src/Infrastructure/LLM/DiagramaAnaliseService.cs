using Application.Contracts.LLM;
using Polly;
using Polly.Retry;

namespace Infrastructure.LLM;

public class DiagramaAnaliseService : IDiagramaAnaliseService
{
    private readonly IDiagramaAnaliseClient _client;

    public DiagramaAnaliseService(IDiagramaAnaliseClient client)
    {
        _client = client;
    }

    public async Task<ResultadoAnaliseDto> AnalisarDiagramaAsync(string nomeFisico, string localizacaoUrl, string extensao)
    {
        var tentativasRealizadas = 0;

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
                    return default;
                }
            })
            .Build();

        try
        {
            var resultado = await pipeline.ExecuteAsync(async cancellationToken =>
            {
                tentativasRealizadas++;
                return await _client.AnalisarDiagramaAsync(nomeFisico, localizacaoUrl, extensao);
            }, CancellationToken.None);

            return resultado with { TentativasRealizadas = tentativasRealizadas };
        }
        catch (Exception ex)
        {
            return new ResultadoAnaliseDto
            {
                Sucesso = false,
                MotivoErro = ex.Message,
                TentativasRealizadas = Math.Max(tentativasRealizadas, 5)
            };
        }
    }
}