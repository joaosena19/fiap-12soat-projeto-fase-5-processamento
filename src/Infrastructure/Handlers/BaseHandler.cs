using Application.Contracts.Monitoramento;
using Infrastructure.Monitoramento;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Handlers;

public abstract class BaseHandler
{
    private readonly ILoggerFactory _loggerFactory;

    protected BaseHandler(ILoggerFactory loggerFactory)
    {
        _loggerFactory = loggerFactory;
    }

    /// <summary>
    /// Fábrica centralizada para criar loggers adaptados para Clean Architecture
    /// </summary>
    protected IAppLogger CriarLoggerPara<TUseCase>()
    {
        return _loggerFactory.CriarAppLogger<TUseCase>();
    }
}
