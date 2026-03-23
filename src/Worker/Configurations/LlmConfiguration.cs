using Application.Contracts.LLM;
using GenerativeAI.Microsoft;
using Infrastructure.LLM;
using Microsoft.Extensions.AI;

namespace Worker.Configurations;

public static class LlmConfiguration
{
    public static IServiceCollection AddLlmServices(this IServiceCollection services, IConfiguration configuration)
    {
        var apiKey = configuration["LLM:ApiKey"]
            ?? throw new InvalidOperationException("LLM:ApiKey não configurado.");
        var model = configuration["LLM:Model"]
            ?? throw new InvalidOperationException("LLM:Model não configurado.");

        services.AddSingleton<IChatClient>(_ => new GenerativeAIChatClient(apiKey, model));

        services.AddScoped<IDiagramaAnaliseClient, LlmDiagramaAnaliseClient>();

        services.AddScoped<IDiagramaAnaliseService, DiagramaAnaliseService>();

        return services;
    }
}