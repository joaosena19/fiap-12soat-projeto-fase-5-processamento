using Infrastructure.Monitoramento;

namespace Tests.Infrastructure.Monitoramento;

public class ContextualLoggerTests
{
    [Fact(DisplayName = "Deve chamar log no logger interno quando invoke LogInformation")]
    [Trait("Infrastructure", "ContextualLogger")]
    public void LogInformation_DeveRepassarParaInnerLogger_QuandoInvocado()
    {
        // Arrange
        var fixture = new ContextualLoggerTestFixture();

        // Act
        fixture.Logger.LogInformation("mensagem {Valor}", "arg1");

        // Assert
        fixture.InnerMock.DeveTerLogadoInformation("mensagem {Valor}");
    }

    [Fact(DisplayName = "Deve chamar LogDebug no logger interno")]
    [Trait("Infrastructure", "ContextualLogger")]
    public void LogDebug_DeveRepassarParaInnerLogger_QuandoInvocado()
    {
        // Arrange
        var fixture = new ContextualLoggerTestFixture();

        // Act
        fixture.Logger.LogDebug("debug {Valor}", "arg");

        // Assert
        fixture.InnerMock.DeveTerLogadoDebug("debug {Valor}");
    }

    [Fact(DisplayName = "Deve chamar LogWarning no logger interno")]
    [Trait("Infrastructure", "ContextualLogger")]
    public void LogWarning_DeveRepassarParaInnerLogger_QuandoInvocado()
    {
        // Arrange
        var fixture = new ContextualLoggerTestFixture();

        // Act
        fixture.Logger.LogWarning("warn {Valor}", "arg");

        // Assert
        fixture.InnerMock.DeveTerLogadoWarning("warn {Valor}");
    }

    [Fact(DisplayName = "Deve chamar LogWarning com exception no logger interno")]
    [Trait("Infrastructure", "ContextualLogger")]
    public void LogWarning_ComException_DeveRepassarParaInnerLogger_QuandoInvocado()
    {
        // Arrange
        var fixture = new ContextualLoggerTestFixture();
        var excecao = new InvalidOperationException("erro");

        // Act
        fixture.Logger.LogWarning(excecao, "warn-ex {Valor}", "arg");

        // Assert
        fixture.InnerMock.DeveTerLogadoWarningComException(excecao, "warn-ex {Valor}");
    }

    [Fact(DisplayName = "Deve chamar LogError no logger interno")]
    [Trait("Infrastructure", "ContextualLogger")]
    public void LogError_DeveRepassarParaInnerLogger_QuandoInvocado()
    {
        // Arrange
        var fixture = new ContextualLoggerTestFixture();

        // Act
        fixture.Logger.LogError("error {Valor}", "arg");

        // Assert
        fixture.InnerMock.DeveTerLogadoError("error {Valor}");
    }

    [Fact(DisplayName = "Deve chamar LogError com exception no logger interno")]
    [Trait("Infrastructure", "ContextualLogger")]
    public void LogError_ComException_DeveRepassarParaInnerLogger_QuandoInvocado()
    {
        // Arrange
        var fixture = new ContextualLoggerTestFixture();
        var excecao = new InvalidOperationException("erro");

        // Act
        fixture.Logger.LogError(excecao, "error-ex {Valor}", "arg");

        // Assert
        fixture.InnerMock.DeveTerLogadoErrorComException(excecao, "error-ex {Valor}");
    }

    [Fact(DisplayName = "ExecutarComContexto deve ignorar chave com valor nulo")]
    [Trait("Infrastructure", "ContextualLogger")]
    public void ExecutarComContexto_DeveIgnorarPropriedadeNula_QuandoValorNulo()
    {
        // Arrange
        var fixture = new ContextualLoggerTestFixture(new Dictionary<string, object?> { ["chaveComValor"] = "valor", ["chaveNula"] = null });

        // Act
        fixture.Logger.LogInformation("mensagem");

        // Assert
        fixture.InnerMock.DeveTerLogadoInformation("mensagem");
    }

    [Fact(DisplayName = "ComPropriedade deve retornar novo contextual logger com chave adicionada")]
    [Trait("Infrastructure", "ContextualLogger")]
    public void ComPropriedade_DeveRetornarNovoContextualLogger_QuandoInvocado()
    {
        // Arrange
        var fixture = new ContextualLoggerTestFixture(new Dictionary<string, object?> { ["key1"] = "val1" });

        // Act
        var novoLogger = fixture.Logger.ComPropriedade("key2", "val2");
        novoLogger.LogInformation("mensagem de verificação");

        // Assert
        novoLogger.ShouldNotBeNull();
        novoLogger.ShouldNotBeSameAs(fixture.Logger);
        novoLogger.ShouldBeOfType<ContextualLogger>();
        fixture.InnerMock.DeveTerLogadoInformation("mensagem de verificação");
    }
}
