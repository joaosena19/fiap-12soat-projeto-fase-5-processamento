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
        fixture.InnerMock.Verify(x => x.LogInformation("mensagem {Valor}", It.IsAny<object[]>()), Times.Once);
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
        fixture.InnerMock.Verify(x => x.LogDebug("debug {Valor}", It.IsAny<object[]>()), Times.Once);
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
        fixture.InnerMock.Verify(x => x.LogWarning("warn {Valor}", It.IsAny<object[]>()), Times.Once);
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
        fixture.InnerMock.Verify(x => x.LogWarning(excecao, "warn-ex {Valor}", It.IsAny<object[]>()), Times.Once);
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
        fixture.InnerMock.Verify(x => x.LogError("error {Valor}", It.IsAny<object[]>()), Times.Once);
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
        fixture.InnerMock.Verify(x => x.LogError(excecao, "error-ex {Valor}", It.IsAny<object[]>()), Times.Once);
    }

    [Fact(DisplayName = "ExecutarComContexto deve ignorar chave com valor nulo")]
    [Trait("Infrastructure", "ContextualLogger")]
    public void ExecutarComContexto_DeveIgnorarPropriedadeNula_QuandoValorNulo()
    {
        // Arrange
        var fixture = new ContextualLoggerTestFixture(new Dictionary<string, object?> { ["chaveComValor"] = "valor", ["chaveNula"] = null });

        // Act
        var acao = () => fixture.Logger.LogInformation("mensagem");

        // Assert
        acao.ShouldNotThrow();
        fixture.InnerMock.Verify(x => x.LogInformation("mensagem", It.IsAny<object[]>()), Times.Once);
    }

    [Fact(DisplayName = "ComPropriedade deve retornar novo contextual logger com chave adicionada")]
    [Trait("Infrastructure", "ContextualLogger")]
    public void ComPropriedade_DeveRetornarNovoContextualLogger_QuandoInvocado()
    {
        // Arrange
        var fixture = new ContextualLoggerTestFixture(new Dictionary<string, object?> { ["key1"] = "val1" });

        // Act
        var novoLogger = fixture.Logger.ComPropriedade("key2", "val2");

        // Assert
        novoLogger.ShouldNotBeNull();
        novoLogger.ShouldNotBeSameAs(fixture.Logger);
        novoLogger.ShouldBeOfType<ContextualLogger>();
    }
}
