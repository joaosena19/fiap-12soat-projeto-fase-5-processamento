using Application.Contracts.Monitoramento;
using Shared.Constants;
using Shared.Exceptions;

namespace Application.Extensions;

public static class LoggerExtensions
{
    public static IAppLogger ComUseCase(this IAppLogger logger, object useCase)
    {
        var useCaseName = useCase.GetType().Name;
        return logger.ComPropriedade(LogNomesPropriedades.UseCase, useCaseName);
    }

    public static IAppLogger ComDomainErrorType(this IAppLogger logger, DomainException ex)
    {
        return logger.ComPropriedade(LogNomesPropriedades.DomainErrorType, ex.ErrorType);
    }
}
