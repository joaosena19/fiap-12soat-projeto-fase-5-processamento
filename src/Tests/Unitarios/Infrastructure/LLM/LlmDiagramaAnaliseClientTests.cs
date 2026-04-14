using Infrastructure.LLM;

namespace Tests.Infrastructure.LLM;

public class LlmDiagramaAnaliseClientTests
{
    private readonly LlmDiagramaAnaliseClientTestFixture _fixture = new();

    [Fact(DisplayName = "Deve retornar sucesso quando LLM confirma diagrama arquitetural com descrição")]
    [Trait("Infrastructure", "LlmDiagramaAnaliseClient")]
    public async Task AnalisarDiagramaAsync_DeveRetornarSucesso_QuandoRespostaEhDiagramaArquitetural()
    {
        // Arrange
        const string json = """{"EhDiagramaArquitetural":true,"DescricaoAnalise":"Análise do sistema","ComponentesIdentificados":["API","DB"],"RiscosArquiteturais":["SPOF"],"RecomendacoesBasicas":["Retry"]}""";
        _fixture.ChatClientMock.AoObterResposta().Retorna(json);

        // Act
        var resultado = await _fixture.AnalisarAsync();

        // Assert
        resultado.Sucesso.ShouldBeTrue();
        resultado.DescricaoAnalise.ShouldBe("Análise do sistema");
        resultado.ComponentesIdentificados.ShouldContain("API");
        resultado.ComponentesIdentificados.ShouldContain("DB");
        resultado.TentativasRealizadas.ShouldBe(1);
    }

    [Fact(DisplayName = "Deve lançar LlmPermanentException quando não é diagrama e MotivoInvalidez está ausente")]
    [Trait("Infrastructure", "LlmDiagramaAnaliseClient")]
    public async Task AnalisarDiagramaAsync_DeveLancarLlmPermanentException_QuandoNaoEhDiagramaSemMotivo()
    {
        // Arrange
        const string json = """{"EhDiagramaArquitetural":false,"MotivoInvalidez":null}""";
        _fixture.ChatClientMock.AoObterResposta().Retorna(json);

        // Act & Assert
        var excecao = await Should.ThrowAsync<LlmPermanentException>(() => _fixture.AnalisarAsync());
        excecao.Message.ShouldContain("MotivoInvalidez");
    }

    [Fact(DisplayName = "Deve retornar falha quando não é diagrama mas MotivoInvalidez está preenchido")]
    [Trait("Infrastructure", "LlmDiagramaAnaliseClient")]
    public async Task AnalisarDiagramaAsync_DeveRetornarFalha_QuandoNaoEhDiagramaComMotivo()
    {
        // Arrange
        const string json = """{"EhDiagramaArquitetural":false,"MotivoInvalidez":"Imagem sem diagrama"}""";
        _fixture.ChatClientMock.AoObterResposta().Retorna(json);

        // Act
        var resultado = await _fixture.AnalisarAsync();

        // Assert
        resultado.DeveSerRejeicao("Imagem sem diagrama");
        resultado.TentativasRealizadas.ShouldBe(1);
    }

    [Fact(DisplayName = "Deve lançar LlmTransientException quando texto da resposta é nulo")]
    [Trait("Infrastructure", "LlmDiagramaAnaliseClient")]
    public async Task AnalisarDiagramaAsync_DeveLancarLlmTransientException_QuandoRespostaNula()
    {
        // Arrange
        _fixture.ChatClientMock.AoObterResposta().RetornaVazio();

        // Act & Assert
        await Should.ThrowAsync<LlmTransientException>(() => _fixture.AnalisarAsync());
    }

    [Fact(DisplayName = "Deve lançar LlmTransientException quando resposta é JSON nulo (falha na desserialização)")]
    [Trait("Infrastructure", "LlmDiagramaAnaliseClient")]
    public async Task AnalisarDiagramaAsync_DeveLancarLlmTransientException_QuandoFalhaDesserializacao()
    {
        // Arrange
        _fixture.ChatClientMock.AoObterResposta().Retorna("null");

        // Act & Assert
        var excecao = await Should.ThrowAsync<LlmTransientException>(() => _fixture.AnalisarAsync());
        excecao.Message.ShouldContain("desserializar");
    }

    [Fact(DisplayName = "Deve lançar LlmTransientException e encapsular exceção genérica do GetResponseAsync")]
    [Trait("Infrastructure", "LlmDiagramaAnaliseClient")]
    public async Task AnalisarDiagramaAsync_DeveLancarLlmTransientException_QuandoExcecaoGenerica()
    {
        // Arrange
        _fixture.ChatClientMock.AoObterResposta().LancaExcecao(new ArgumentException("Argumento inválido no chat client"));

        // Act & Assert
        var excecao = await Should.ThrowAsync<LlmTransientException>(() => _fixture.AnalisarAsync());
        excecao.Message.ShouldContain("transitória");
        excecao.InnerException.ShouldBeOfType<ArgumentException>();
    }

    [Fact(DisplayName = "Deve relançar LlmPermanentException sem encapsular quando já é permanente")]
    [Trait("Infrastructure", "LlmDiagramaAnaliseClient")]
    public async Task AnalisarDiagramaAsync_DeveRelancaLlmPermanentException_QuandoJaEPermanent()
    {
        // Arrange
        _fixture.ChatClientMock.AoObterResposta().LancaExcecao(new LlmPermanentException("Erro permanente direto"));

        // Act & Assert
        var excecao = await Should.ThrowAsync<LlmPermanentException>(() => _fixture.AnalisarAsync());
        excecao.Message.ShouldBe("Erro permanente direto");
        excecao.InnerException.ShouldBeNull();
    }

    [Fact(DisplayName = "Deve relançar LlmTransientException sem encapsular quando já é transitória")]
    [Trait("Infrastructure", "LlmDiagramaAnaliseClient")]
    public async Task AnalisarDiagramaAsync_DeveRelancaLlmTransientException_QuandoJaETransient()
    {
        // Arrange
        _fixture.ChatClientMock.AoObterResposta().LancaExcecao(new LlmTransientException("Timeout na LLM"));

        // Act & Assert
        var excecao = await Should.ThrowAsync<LlmTransientException>(() => _fixture.AnalisarAsync());
        excecao.Message.ShouldBe("Timeout na LLM");
        excecao.InnerException.ShouldBeNull();
    }

    [Fact(DisplayName = "Deve lançar LlmPermanentException quando diagrama é válido mas DescricaoAnalise está ausente")]
    [Trait("Infrastructure", "LlmDiagramaAnaliseClient")]
    public async Task AnalisarDiagramaAsync_DeveLancarLlmPermanentException_QuandoDiagramaValidoSemDescricao()
    {
        // Arrange
        const string json = """{"EhDiagramaArquitetural":true,"DescricaoAnalise":null}""";
        _fixture.ChatClientMock.AoObterResposta().Retorna(json);

        // Act & Assert
        var excecao = await Should.ThrowAsync<LlmPermanentException>(() => _fixture.AnalisarAsync());
        excecao.Message.ShouldContain("DescricaoAnalise");
    }

    [Theory(DisplayName = "Deve processar corretamente arquivos com diferentes extensões")]
    [InlineData(".png")]
    [InlineData(".jpg")]
    [InlineData(".jpeg")]
    [InlineData(".gif")]
    [InlineData(".webp")]
    [InlineData(".pdf")]
    [InlineData(".bmp")]
    [InlineData(".desconhecida")]
    [Trait("Infrastructure", "LlmDiagramaAnaliseClient")]
    public async Task AnalisarDiagramaAsync_DeveProcessar_ParaDiferentesExtensoes(string extensao)
    {
        // Arrange
        const string json = """{"EhDiagramaArquitetural":true,"DescricaoAnalise":"OK","ComponentesIdentificados":[],"RiscosArquiteturais":[],"RecomendacoesBasicas":[]}""";
        _fixture.ChatClientMock.AoObterResposta().Retorna(json);

        // Act
        var resultado = await _fixture.AnalisarAsync("arquivo" + extensao, extensao: extensao);

        // Assert
        resultado.Sucesso.ShouldBeTrue();
    }
}
