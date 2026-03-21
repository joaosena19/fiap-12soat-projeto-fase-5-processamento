using Application.Contracts.LLM;
using Infrastructure.LLM;

namespace Worker.Configurations;

public static class LlmConfiguration
{
    public static IServiceCollection AddLlmServices(this IServiceCollection services)
    {
        services.AddScoped<IDiagramaAnaliseClient, PlaceholderDiagramaAnaliseService>();
        services.AddScoped<IDiagramaAnaliseService, DiagramaAnaliseService>();

        return services;
    }
}