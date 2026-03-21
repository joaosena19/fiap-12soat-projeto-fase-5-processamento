using Application.Contracts.Gateways;
using Application.Contracts.Monitoramento;
using Infrastructure.Monitoramento;
using Infrastructure.Repositories;

namespace Worker.Configurations;

public static class MonitoringConfiguration
{
    public static IServiceCollection AddMonitoring(this IServiceCollection services)
    {
        services.AddScoped<ICorrelationIdAccessor, CorrelationIdAccessor>();
        services.AddSingleton<IMetricsService, NewRelicMetricsService>();
        services.AddScoped<IProcessamentoDiagramaGateway, ProcessamentoDiagramaRepository>();

        return services;
    }
}