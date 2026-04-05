using Infrastructure.Monitoramento;
using Microsoft.Extensions.Logging;

namespace Tests.Infrastructure.Monitoramento;

public class LoggerAdapterTests
{
    [Fact(DisplayName = "Métodos de log não devem lançar exceção")]
    [Trait("Infrastructure", "LoggerAdapter")]
    public void Logs_NaoDevemLancarExcecao_QuandoInvocados()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<object>>();
        var adapter = new LoggerAdapter<object>(loggerMock.Object);

        // Act
        var acao = () =>
        {
            adapter.LogDebug("debug {Valor}", 1);
            adapter.LogInformation("info {Valor}", 2);
            adapter.LogWarning("warn {Valor}", 3);
            adapter.LogWarning(new InvalidOperationException("warn"), "warn-ex {Valor}", 4);
            adapter.LogError("error {Valor}", 5);
            adapter.LogError(new InvalidOperationException("error"), "error-ex {Valor}", 6);
        };

        // Assert
        acao.ShouldNotThrow();
    }

    [Fact(DisplayName = "ComPropriedade deve retornar logger contextualizado")]
    [Trait("Infrastructure", "LoggerAdapter")]
    public void ComPropriedade_DeveRetornarContextualizado_QuandoInvocado()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<object>>();
        var adapter = new LoggerAdapter<object>(loggerMock.Object);

        // Act
        var contextualizado = adapter.ComPropriedade("Chave", "Valor");

        // Assert
        contextualizado.ShouldNotBeNull();
        contextualizado.ShouldNotBeSameAs(adapter);
    }

    [Fact(DisplayName = "ComPropriedade deve aceitar valor nulo")]
    [Trait("Infrastructure", "LoggerAdapter")]
    public void ComPropriedade_DeveAceitarValorNulo_QuandoNullPassado()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<object>>();
        var adapter = new LoggerAdapter<object>(loggerMock.Object);

        // Act
        var resultado = () => adapter.ComPropriedade("Chave", null);

        // Assert
        resultado.ShouldNotThrow();
    }
}
