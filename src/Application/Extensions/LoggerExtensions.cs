using Application.Contracts.Monitoramento;
using Shared.Constants;
using Shared.Exceptions;

namespace Application.Extensions;

public static class LoggerExtensions
{
    const string TipoMensageriaNome_Envio = "Envio";
    const string TipoMensageriaNome_Consumo = "Consumo";

    public static IAppLogger ComUseCase(this IAppLogger logger, object useCase)
    {
        var useCaseName = useCase.GetType().Name;
        return logger.ComPropriedade(LogNomesPropriedades.UseCase, useCaseName);
    }

    public static IAppLogger ComEnvioMensagem(this IAppLogger logger, object mensageria)
    {
        var nomeMensageria = mensageria.GetType().Name;
        return logger.ComPropriedade(LogNomesPropriedades.Mensageria, nomeMensageria).ComPropriedade(LogNomesPropriedades.TipoMensageria, TipoMensageriaNome_Envio);
    }

    public static IAppLogger ComConsumoMensagem(this IAppLogger logger, object mensageria)
    {
        var nomeMensageria = mensageria.GetType().Name;
        return logger.ComPropriedade(LogNomesPropriedades.Mensageria, nomeMensageria).ComPropriedade(LogNomesPropriedades.TipoMensageria, TipoMensageriaNome_Consumo);
    }

    public static IAppLogger ComDomainErrorType(this IAppLogger logger, DomainException ex)
    {
        return logger.ComPropriedade(LogNomesPropriedades.DomainErrorType, ex.ErrorType);
    }
}
