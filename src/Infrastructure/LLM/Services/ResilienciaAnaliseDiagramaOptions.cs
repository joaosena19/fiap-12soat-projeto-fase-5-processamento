using Microsoft.Extensions.Configuration;

namespace Infrastructure.LLM;

internal sealed class ResilienciaAnaliseDiagramaOptions
{
    private const int MaxTentativasPorModeloPadrao = 2;
    private const int DelaySegundosPadrao = 3;

    public int MaxTentativasPorModelo { get; }
    public int DelaySegundos { get; }

    private ResilienciaAnaliseDiagramaOptions(int maxTentativasPorModelo, int delaySegundos)
    {
        MaxTentativasPorModelo = maxTentativasPorModelo;
        DelaySegundos = delaySegundos;
    }

    public static ResilienciaAnaliseDiagramaOptions Criar(IConfiguration configuration)
    {
        var maxTentativasPorModelo = int.TryParse(configuration["Resiliencia:LLM:MaxTentativasPorModelo"], out var valor) ? valor
            : int.TryParse(configuration["Resiliencia:LLM:MaxTentativas"], out var valorLegado) ? valorLegado
            : MaxTentativasPorModeloPadrao;
        var delaySegundos = int.TryParse(configuration["Resiliencia:LLM:DelaySegundos"], out var delaySegundosConfigurados) ? delaySegundosConfigurados : DelaySegundosPadrao;

        return new ResilienciaAnaliseDiagramaOptions(maxTentativasPorModelo, delaySegundos);
    }
}