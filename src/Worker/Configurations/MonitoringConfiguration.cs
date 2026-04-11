using Application.Contracts.Monitoramento;
using Infrastructure.Monitoramento;

namespace Worker.Configurations;

public static class MonitoringConfiguration
{
    public static IServiceCollection AddMonitoring(this IServiceCollection services)
    {
        services.AddScoped<ICorrelationIdAccessor, CorrelationIdAccessor>();

        return services;
    }
}