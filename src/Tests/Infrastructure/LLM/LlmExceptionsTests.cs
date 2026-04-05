using Infrastructure.LLM;

namespace Tests.Infrastructure.LLM;

public class LlmExceptionsTests
{
    #region LlmPermanentException

    [Fact(DisplayName = "Deve criar LlmPermanentException com mensagem")]
    [Trait("Infrastructure", "LlmPermanentException")]
    public void LlmPermanentException_DeveCriarComMensagem()
    {
        // Act
        var excecao = new LlmPermanentException("Erro permanente");

        // Assert
        excecao.Message.ShouldBe("Erro permanente");
        excecao.InnerException.ShouldBeNull();
        excecao.ShouldBeAssignableTo<Exception>();
    }

    [Fact(DisplayName = "Deve criar LlmPermanentException com mensagem e exceção interna")]
    [Trait("Infrastructure", "LlmPermanentException")]
    public void LlmPermanentException_DeveCriarComMensagemEExcecaoInterna()
    {
        // Arrange
        var excecaoInterna = new InvalidOperationException("Causa raiz");

        // Act
        var excecao = new LlmPermanentException("Erro permanente com causa", excecaoInterna);

        // Assert
        excecao.Message.ShouldBe("Erro permanente com causa");
        excecao.InnerException.ShouldBeSameAs(excecaoInterna);
        excecao.InnerException!.Message.ShouldBe("Causa raiz");
    }

    #endregion

    #region LlmTransientException

    [Fact(DisplayName = "Deve criar LlmTransientException com mensagem")]
    [Trait("Infrastructure", "LlmTransientException")]
    public void LlmTransientException_DeveCriarComMensagem()
    {
        // Act
        var excecao = new LlmTransientException("Erro transitório");

        // Assert
        excecao.Message.ShouldBe("Erro transitório");
        excecao.InnerException.ShouldBeNull();
        excecao.ShouldBeAssignableTo<Exception>();
    }

    [Fact(DisplayName = "Deve criar LlmTransientException com mensagem e exceção interna")]
    [Trait("Infrastructure", "LlmTransientException")]
    public void LlmTransientException_DeveCriarComMensagemEExcecaoInterna()
    {
        // Arrange
        var excecaoInterna = new TimeoutException("Timeout de rede");

        // Act
        var excecao = new LlmTransientException("Falha transitória ao consultar LLM", excecaoInterna);

        // Assert
        excecao.Message.ShouldBe("Falha transitória ao consultar LLM");
        excecao.InnerException.ShouldBeSameAs(excecaoInterna);
        excecao.InnerException!.Message.ShouldBe("Timeout de rede");
    }

    [Fact(DisplayName = "LlmTransientException e LlmPermanentException devem ser tipos distintos")]
    [Trait("Infrastructure", "LlmTransientException")]
    public void LlmTransientException_NaoDeveSerLlmPermanentException()
    {
        // Act
        var transiente = new LlmTransientException("T");
        var permanente = new LlmPermanentException("P");

        // Assert
        transiente.ShouldNotBeAssignableTo<LlmPermanentException>();
        permanente.ShouldNotBeAssignableTo<LlmTransientException>();
    }

    #endregion

    #region LlmAnaliseResponse

    [Fact(DisplayName = "Deve inicializar LlmAnaliseResponse com valores padrão")]
    [Trait("Infrastructure", "LlmAnaliseResponse")]
    public void LlmAnaliseResponse_DeveInicializarComValoresPadrao()
    {
        // Act
        var resposta = new LlmAnaliseResponse();

        // Assert
        resposta.EhDiagramaArquitetural.ShouldBeFalse();
        resposta.MotivoInvalidez.ShouldBeNull();
        resposta.DescricaoAnalise.ShouldBeNull();
        resposta.ComponentesIdentificados.ShouldNotBeNull();
        resposta.ComponentesIdentificados.ShouldBeEmpty();
        resposta.RiscosArquiteturais.ShouldNotBeNull();
        resposta.RiscosArquiteturais.ShouldBeEmpty();
        resposta.RecomendacoesBasicas.ShouldNotBeNull();
        resposta.RecomendacoesBasicas.ShouldBeEmpty();
    }

    [Fact(DisplayName = "Deve atribuir e ler todas as propriedades de LlmAnaliseResponse")]
    [Trait("Infrastructure", "LlmAnaliseResponse")]
    public void LlmAnaliseResponse_DeveAtribuirTodasAsPropriedades()
    {
        // Arrange & Act
        var resposta = new LlmAnaliseResponse
        {
            EhDiagramaArquitetural = true,
            MotivoInvalidez = null,
            DescricaoAnalise = "Diagrama de microsserviços",
            ComponentesIdentificados = ["API Gateway", "Serviço de Pagamento"],
            RiscosArquiteturais = ["Ponto único de falha"],
            RecomendacoesBasicas = ["Implementar circuit breaker"]
        };

        // Assert
        resposta.EhDiagramaArquitetural.ShouldBeTrue();
        resposta.MotivoInvalidez.ShouldBeNull();
        resposta.DescricaoAnalise.ShouldBe("Diagrama de microsserviços");
        resposta.ComponentesIdentificados.ShouldContain("API Gateway");
        resposta.ComponentesIdentificados.ShouldContain("Serviço de Pagamento");
        resposta.RiscosArquiteturais.ShouldContain("Ponto único de falha");
        resposta.RecomendacoesBasicas.ShouldContain("Implementar circuit breaker");
    }

    [Fact(DisplayName = "Deve criar LlmAnaliseResponse com diagrama inválido e motivo")]
    [Trait("Infrastructure", "LlmAnaliseResponse")]
    public void LlmAnaliseResponse_DeveCriarComDiagramaInvalidoEMotivo()
    {
        // Arrange & Act
        var resposta = new LlmAnaliseResponse
        {
            EhDiagramaArquitetural = false,
            MotivoInvalidez = "Imagem não contém diagrama arquitetural"
        };

        // Assert
        resposta.EhDiagramaArquitetural.ShouldBeFalse();
        resposta.MotivoInvalidez.ShouldBe("Imagem não contém diagrama arquitetural");
        resposta.DescricaoAnalise.ShouldBeNull();
    }

    #endregion
}
