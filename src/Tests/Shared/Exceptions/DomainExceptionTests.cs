namespace Tests.Shared.Exceptions;

public class DomainExceptionTests
{
    #region Construtor Simples

    [Fact(DisplayName = "Deve criar com mensagem padrão quando sem argumentos")]
    [Trait("Shared", "DomainException")]
    public void Construtor_DeveUsarMensagemPadrao_QuandoSemArgumentos()
    {
        // Act
        var excecao = new DomainException();

        // Assert
        excecao.Message.ShouldBe("Invalid input");
        excecao.ErrorType.ShouldBe(ErrorType.InvalidInput);
        excecao.LogTemplate.ShouldBe("Invalid input");
        excecao.LogArgs.ShouldBeEmpty();
    }

    [Fact(DisplayName = "Deve criar com mensagem e ErrorType padrão")]
    [Trait("Shared", "DomainException")]
    public void Construtor_DeveCriarComMensagem_QuandoSomenteComMensagem()
    {
        // Act
        var excecao = new DomainException("mensagem de erro");

        // Assert
        excecao.Message.ShouldBe("mensagem de erro");
        excecao.ErrorType.ShouldBe(ErrorType.InvalidInput);
        excecao.LogTemplate.ShouldBe("mensagem de erro");
        excecao.LogArgs.ShouldBeEmpty();
    }

    [Theory(DisplayName = "Deve criar com ErrorType especificado")]
    [InlineData(ErrorType.ResourceNotFound)]
    [InlineData(ErrorType.DomainRuleBroken)]
    [InlineData(ErrorType.Conflict)]
    [InlineData(ErrorType.NotAllowed)]
    [InlineData(ErrorType.UnexpectedError)]
    [Trait("Shared", "DomainException")]
    public void Construtor_DeveCriarComErrorType_QuandoEspecificado(ErrorType errorType)
    {
        // Act
        var excecao = new DomainException("mensagem", errorType);

        // Assert
        excecao.ErrorType.ShouldBe(errorType);
        excecao.LogTemplate.ShouldBe("mensagem");
        excecao.LogArgs.ShouldBeEmpty();
    }

    #endregion

    #region Construtor Completo

    [Fact(DisplayName = "Deve preencher LogTemplate e LogArgs no construtor completo")]
    [Trait("Shared", "DomainException")]
    public void Construtor_DevePreencherLogTemplateELogArgs_QuandoConstrutorCompleto()
    {
        // Arrange
        var mensagemUsuario = "Recurso não encontrado";
        var logTemplate = "Recurso {Id} não encontrado para usuário {Usuario}";
        var id = Guid.NewGuid();
        var usuario = "joao";

        // Act
        var excecao = new DomainException(mensagemUsuario, ErrorType.ResourceNotFound, logTemplate, id, usuario);

        // Assert
        excecao.Message.ShouldBe(mensagemUsuario);
        excecao.ErrorType.ShouldBe(ErrorType.ResourceNotFound);
        excecao.LogTemplate.ShouldBe(logTemplate);
        excecao.LogArgs.Length.ShouldBe(2);
        excecao.LogArgs[0].ShouldBe(id);
        excecao.LogArgs[1].ShouldBe(usuario);
    }

    [Fact(DisplayName = "Deve criar com LogArgs vazio no construtor completo sem args")]
    [Trait("Shared", "DomainException")]
    public void Construtor_DeveLogArgsVazio_QuandoConstrutorCompletoSemArgs()
    {
        // Act
        var excecao = new DomainException("mensagem", ErrorType.DomainRuleBroken, "template sem args");

        // Assert
        excecao.LogTemplate.ShouldBe("template sem args");
        excecao.LogArgs.ShouldBeEmpty();
    }

    [Fact(DisplayName = "Deve herdar de Exception")]
    [Trait("Shared", "DomainException")]
    public void DomainException_DeveHerdarDeException()
    {
        // Act
        var excecao = new DomainException("mensagem");

        // Assert
        excecao.ShouldBeAssignableTo<Exception>();
    }

    #endregion
}
