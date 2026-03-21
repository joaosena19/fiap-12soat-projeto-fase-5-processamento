using Shared.Enums;

namespace Shared.Exceptions;

public class DomainException : Exception
{
    public ErrorType ErrorType { get; }
    public string LogTemplate { get; }
    public object[] LogArgs { get; }

    public DomainException(string message = "Invalid input", ErrorType errorType = ErrorType.InvalidInput)
        : base(message)
    {
        ErrorType = errorType;
        LogTemplate = message;
        LogArgs = Array.Empty<object>();
    }

    public DomainException(string mensagemUsuario, ErrorType errorType, string logTemplate, params object[] logArgs)
        : base(mensagemUsuario)
    {
        ErrorType = errorType;
        LogTemplate = logTemplate;
        LogArgs = logArgs;
    }
}
