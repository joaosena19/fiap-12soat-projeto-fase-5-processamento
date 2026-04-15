using Application.Contracts.LLM;
using Infrastructure.LLM;
using Serilog;

namespace Worker.Configurations;

public static class LlmConfiguration
{
    public static IServiceCollection AddLlmServices(this IServiceCollection services, IConfiguration configuration)
    {
        var opcoes = configuration.GetSection("LLM").Get<LlmOptions>();

        if (opcoes is null || string.IsNullOrWhiteSpace(opcoes.ApiKey))
        {
            Log.Error("LLM:ApiKey não configurado. O serviço não conseguirá processar diagramas.");
            throw new InvalidOperationException("LLM:ApiKey não configurado.");
        }

        if (opcoes.Modelos is null || opcoes.Modelos.Count == 0)
        {
            Log.Error("LLM:Modelos não configurado ou vazio. É necessário ao menos um modelo.");
            throw new InvalidOperationException("LLM:Modelos não configurado ou vazio.");
        }

        services.AddSingleton(opcoes);
        services.AddScoped<ILlmClientFactory, LlmClientFactory>();
        services.AddScoped<IDiagramaAnaliseService, DiagramaAnaliseService>();

        return services;
    }
}