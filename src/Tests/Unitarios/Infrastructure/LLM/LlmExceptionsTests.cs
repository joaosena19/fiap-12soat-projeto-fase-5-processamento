using Infrastructure.LLM;
using System.Text.Json;

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

    [Fact(DisplayName = "LlmTransientException e LlmPermanentException devem ser tipos distintos em runtime")]
    [Trait("Infrastructure", "LlmTransientException")]
    public void LlmTransientException_DeveTerTipoDistintoDeLlmPermanentException()
    {
        // Act
        var transiente = new LlmTransientException("Falha transitória");
        var permanente = new LlmPermanentException("Falha permanente");

        // Assert
        transiente.ShouldBeAssignableTo<Exception>();
        permanente.ShouldBeAssignableTo<Exception>();
        transiente.GetType().ShouldNotBe(permanente.GetType());
    }

    #endregion

    #region LlmAnaliseResponse

    [Fact(DisplayName = "Deve desserializar LlmAnaliseResponse de JSON com diagrama arquitetural válido")]
    [Trait("Infrastructure", "LlmAnaliseResponse")]
    public void LlmAnaliseResponse_DeveDesserializarDeJson_QuandoDiagramaArquitetural()
    {
        // Arrange
        const string json = """
        {
            "EhDiagramaArquitetural": true,
            "DescricaoAnalise": "Arquitetura de microsserviços com API Gateway",
            "ComponentesIdentificados": ["API Gateway", "Serviço de Pagamento"],
            "RiscosArquiteturais": ["Ponto único de falha no gateway"],
            "RecomendacoesBasicas": ["Adicionar circuit breaker"]
        }
        """;
        var opcoes = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        // Act
        var resposta = JsonSerializer.Deserialize<LlmAnaliseResponse>(json, opcoes);

        // Assert
        resposta.ShouldNotBeNull();
        resposta.EhDiagramaArquitetural.ShouldBeTrue();
        resposta.DescricaoAnalise.ShouldBe("Arquitetura de microsserviços com API Gateway");
        resposta.ComponentesIdentificados.ShouldContain("API Gateway");
        resposta.ComponentesIdentificados.ShouldContain("Serviço de Pagamento");
        resposta.RiscosArquiteturais.ShouldContain("Ponto único de falha no gateway");
        resposta.RecomendacoesBasicas.ShouldContain("Adicionar circuit breaker");
    }

    [Fact(DisplayName = "Deve desserializar LlmAnaliseResponse de JSON com diagrama inválido")]
    [Trait("Infrastructure", "LlmAnaliseResponse")]
    public void LlmAnaliseResponse_DeveDesserializarDeJson_QuandoDiagramaInvalido()
    {
        // Arrange
        const string json = """
        {
            "EhDiagramaArquitetural": false,
            "MotivoInvalidez": "Imagem não contém diagrama arquitetural"
        }
        """;
        var opcoes = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        // Act
        var resposta = JsonSerializer.Deserialize<LlmAnaliseResponse>(json, opcoes);

        // Assert
        resposta.ShouldNotBeNull();
        resposta.EhDiagramaArquitetural.ShouldBeFalse();
        resposta.MotivoInvalidez.ShouldBe("Imagem não contém diagrama arquitetural");
        resposta.ComponentesIdentificados.ShouldNotBeNull();
        resposta.ComponentesIdentificados.ShouldBeEmpty();
    }

    #endregion

    #region LlmIndisponivelException

    [Fact(DisplayName = "Deve criar LlmIndisponivelException com modelo, código HTTP e mensagem")]
    [Trait("Infrastructure", "LlmIndisponivelException")]
    public void LlmIndisponivelException_DeveCriarComModeloCodigoHttpEMensagem()
    {
        // Act
        var excecao = new LlmIndisponivelException("gemini-2.5-flash", 429, "Quota excedida");

        // Assert
        excecao.Modelo.ShouldBe("gemini-2.5-flash");
        excecao.CodigoHttp.ShouldBe(429);
        excecao.Message.ShouldBe("Quota excedida");
        excecao.InnerException.ShouldBeNull();
    }

    [Fact(DisplayName = "Deve criar LlmIndisponivelException com exceção interna")]
    [Trait("Infrastructure", "LlmIndisponivelException")]
    public void LlmIndisponivelException_DeveCriarComExcecaoInterna()
    {
        // Arrange
        var excecaoInterna = new HttpRequestException("rate limited");

        // Act
        var excecao = new LlmIndisponivelException("gemini-2.5-flash", 429, "Quota excedida", excecaoInterna);

        // Assert
        excecao.InnerException.ShouldBeSameAs(excecaoInterna);
        excecao.CodigoHttp.ShouldBe(429);
    }

    [Fact(DisplayName = "LlmIndisponivelException deve herdar de LlmTransientException")]
    [Trait("Infrastructure", "LlmIndisponivelException")]
    public void LlmIndisponivelException_DeveHerdarDeLlmTransientException()
    {
        // Act
        var excecao = new LlmIndisponivelException("gemini-test", 503, "Serviço indisponível");

        // Assert
        excecao.ShouldBeAssignableTo<LlmTransientException>();
    }

    [Fact(DisplayName = "LlmIndisponivelException deve ser distinguível de LlmTransientException em runtime")]
    [Trait("Infrastructure", "LlmIndisponivelException")]
    public void LlmIndisponivelException_DeveSerDistinguivelDeLlmTransientException()
    {
        // Arrange
        var indisponivel = new LlmIndisponivelException("gemini-test", 429, "Quota");
        var transiente = new LlmTransientException("Timeout");

        // Assert
        (indisponivel is LlmIndisponivelException).ShouldBeTrue();
        (transiente is LlmIndisponivelException).ShouldBeFalse();
        (indisponivel is LlmTransientException).ShouldBeTrue();
    }

    #endregion
}
