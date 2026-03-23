using Application.Contracts.Monitoramento;
using Serilog.Context;

namespace Infrastructure.Monitoramento;

public class ContextualLogger : IAppLogger
{
    private readonly IAppLogger _innerLogger;
    private readonly Dictionary<string, object?> _context;

    public ContextualLogger(IAppLogger innerLogger, Dictionary<string, object?> context)
    {
        _innerLogger = innerLogger;
        _context = context;
    }

    private void ExecutarComContexto(Action logAction)
    {
        var disposables = new List<IDisposable>();

        foreach (var kvp in _context)
        {
            if (kvp.Value != null)
                disposables.Add(LogContext.PushProperty(kvp.Key, kvp.Value));
        }

        try
        {
            logAction();
        }
        finally
        {
            foreach (var disposable in disposables)
                disposable.Dispose();
        }
    }

    public void LogDebug(string messageTemplate, params object[] args) => ExecutarComContexto(() => _innerLogger.LogDebug(messageTemplate, args));

    public void LogInformation(string messageTemplate, params object[] args) => ExecutarComContexto(() => _innerLogger.LogInformation(messageTemplate, args));

    public void LogWarning(string messageTemplate, params object[] args) => ExecutarComContexto(() => _innerLogger.LogWarning(messageTemplate, args));

    public void LogWarning(Exception ex, string messageTemplate, params object[] args) => ExecutarComContexto(() => _innerLogger.LogWarning(ex, messageTemplate, args));

    public void LogError(string messageTemplate, params object[] args) => ExecutarComContexto(() => _innerLogger.LogError(messageTemplate, args));

    public void LogError(Exception ex, string messageTemplate, params object[] args) => ExecutarComContexto(() => _innerLogger.LogError(ex, messageTemplate, args));

    public IAppLogger ComPropriedade(string key, object? value)
    {
        var newContext = new Dictionary<string, object?>(_context) { [key] = value };
        return new ContextualLogger(_innerLogger, newContext);
    }
}
