using Infrastructure.Monitoramento;
using Microsoft.Extensions.Logging;

namespace Tests.Infrastructure.Monitoramento;

public class LoggerAdapterTests
{
    private readonly Mock<ILogger<object>> _loggerMock = new();

    private LoggerAdapter<object> CriarAdapter() => new(_loggerMock.Object);

    [Fact(DisplayName = "LogDebug deve chamar ILogger")]
    [Trait("Infrastructure", "LoggerAdapter")]
    public void LogDebug_DeveChamarILogger_QuandoInvocado()
    {
        // Act
        CriarAdapter().LogDebug("debug {Valor}", 1);

        // Assert
        _loggerMock.DeveTerLogadoDebug();
    }

    [Fact(DisplayName = "LogInformation deve chamar ILogger")]
    [Trait("Infrastructure", "LoggerAdapter")]
    public void LogInformation_DeveChamarILogger_QuandoInvocado()
    {
        // Act
        CriarAdapter().LogInformation("info {Valor}", 2);

        // Assert
        _loggerMock.DeveTerLogadoInformation();
    }

    [Fact(DisplayName = "LogWarning deve chamar ILogger")]
    [Trait("Infrastructure", "LoggerAdapter")]
    public void LogWarning_DeveChamarILogger_QuandoInvocado()
    {
        // Act
        CriarAdapter().LogWarning("warn {Valor}", 3);

        // Assert
        _loggerMock.DeveTerLogadoWarning();
    }

    [Fact(DisplayName = "LogWarning com exceção deve chamar ILogger")]
    [Trait("Infrastructure", "LoggerAdapter")]
    public void LogWarning_ComException_DeveChamarILogger_QuandoInvocado()
    {
        // Arrange
        var excecao = new InvalidOperationException("warn");

        // Act
        CriarAdapter().LogWarning(excecao, "warn-ex {Valor}", 4);

        // Assert
        _loggerMock.DeveTerLogadoWarningComException(excecao);
    }

    [Fact(DisplayName = "LogError deve chamar ILogger")]
    [Trait("Infrastructure", "LoggerAdapter")]
    public void LogError_DeveChamarILogger_QuandoInvocado()
    {
        // Act
        CriarAdapter().LogError("error {Valor}", 5);

        // Assert
        _loggerMock.DeveTerLogadoError();
    }

    [Fact(DisplayName = "LogError com exceção deve chamar ILogger")]
    [Trait("Infrastructure", "LoggerAdapter")]
    public void LogError_ComException_DeveChamarILogger_QuandoInvocado()
    {
        // Arrange
        var excecao = new InvalidOperationException("error");

        // Act
        CriarAdapter().LogError(excecao, "error-ex {Valor}", 6);

        // Assert
        _loggerMock.DeveTerLogadoErrorComException(excecao);
    }

    [Fact(DisplayName = "ComPropriedade deve retornar logger contextualizado")]
    [Trait("Infrastructure", "LoggerAdapter")]
    public void ComPropriedade_DeveRetornarContextualizado_QuandoInvocado()
    {
        // Arrange
        var adapter = CriarAdapter();

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
        var adapter = CriarAdapter();

        // Act
        var contextualizado = adapter.ComPropriedade("Chave", null);

        // Assert
        contextualizado.ShouldNotBeNull();
        contextualizado.ShouldNotBeSameAs(adapter);
    }
}
