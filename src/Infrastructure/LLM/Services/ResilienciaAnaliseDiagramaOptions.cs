using Microsoft.Extensions.Configuration;

namespace Infrastructure.LLM;

internal sealed class ResilienciaAnaliseDiagramaOptions
{
    private const int MaxTentativasPadrao = 4;
    private const int DelaySegundosPadrao = 1;

    public int MaxTentativas { get; }
    public int DelaySegundos { get; }

    private ResilienciaAnaliseDiagramaOptions(int maxTentativas, int delaySegundos)
    {
        MaxTentativas = maxTentativas;
        DelaySegundos = delaySegundos;
    }

    public static ResilienciaAnaliseDiagramaOptions Criar(IConfiguration configuration)
    {
        var maxTentativas = int.TryParse(configuration["Resiliencia:LLM:MaxTentativas"], out var maxTentativasConfiguradas) ? maxTentativasConfiguradas : MaxTentativasPadrao;
        var delaySegundos = int.TryParse(configuration["Resiliencia:LLM:DelaySegundos"], out var delaySegundosConfigurados) ? delaySegundosConfigurados : DelaySegundosPadrao;

        return new ResilienciaAnaliseDiagramaOptions(maxTentativas, delaySegundos);
    }
}