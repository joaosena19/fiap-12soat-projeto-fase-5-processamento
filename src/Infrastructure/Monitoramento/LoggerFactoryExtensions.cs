using Application.Contracts.Monitoramento;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Monitoramento;

/// <summary>
/// Extensões para criação simplificada de loggers adaptados para Clean Architecture.
/// </summary>
public static class LoggerFactoryExtensions
{
    public static IAppLogger CriarAppLogger<T>(this ILoggerFactory loggerFactory) => new LoggerAdapter<T>(loggerFactory.CreateLogger<T>());
}
