using Tests.Helpers.Fixtures;
using Tests.Helpers.MockExtensions;
using Infrastructure.LLM;

namespace Tests.Infrastructure.LLM;

public class DiagramaAnaliseServiceTests
{
    [Fact(DisplayName = "Deve retornar sucesso quando análise é concluída no primeiro modelo")]
    [Trait("Infrastructure", "DiagramaAnaliseService")]
    public async Task AnalisarDiagramaAsync_DeveRetornarSucesso_QuandoAnaliseConcluida()
    {
        // Arrange
        var fixture = new DiagramaAnaliseServiceTestFixture(maxTentativasPorModelo: 1, delaySegundos: 0);
        var resultadoEsperado = new ResultadoAnaliseDtoBuilder().Sucesso().Build();
        var clientMock = fixture.ClientFactoryMock.AoCriarPara("modelo-principal").RetornaMock();

        fixture.DownloaderMock.AoBaixar().Retorna([0x01, 0x02]);
        clientMock.AoAnalisar().Retorna(resultadoEsperado);

        // Act
        var resultado = await fixture.Service.AnalisarDiagramaAsync(Guid.NewGuid(), "arquivo.png", "https://bucket/arquivo.png", ".png");

        // Assert
        resultado.DeveSerSucesso(1);
        fixture.ClientFactoryMock.NaoDeveTerCriadoPara("modelo-fallback");
    }

    [Fact(DisplayName = "Deve retornar falha quando download do arquivo gera exceção")]
    [Trait("Infrastructure", "DiagramaAnaliseService")]
    public async Task AnalisarDiagramaAsync_DeveRetornarFalha_QuandoDownloadFalha()
    {
        // Arrange
        var fixture = new DiagramaAnaliseServiceTestFixture(maxTentativasPorModelo: 1, delaySegundos: 0);

        fixture.DownloaderMock.AoBaixar().LancaExcecao(new InvalidOperationException("Falha no download"));

        // Act
        var resultado = await fixture.Service.AnalisarDiagramaAsync(Guid.NewGuid(), "arquivo.png", "https://bucket/arquivo.png", ".png");

        // Assert
        resultado.DeveSerFalha("Falha no download", 0);
    }

    [Fact(DisplayName = "Deve fazer fallback para segundo modelo quando primeiro retorna 429")]
    [Trait("Infrastructure", "DiagramaAnaliseService")]
    public async Task AnalisarDiagramaAsync_DeveFazerFallback_QuandoPrimeiroModeloRetorna429()
    {
        // Arrange
        var fixture = new DiagramaAnaliseServiceTestFixture(maxTentativasPorModelo: 1, delaySegundos: 0);
        var resultadoEsperado = new ResultadoAnaliseDtoBuilder().Sucesso().Build();

        var clientMockA = fixture.ClientFactoryMock.AoCriarPara("modelo-principal").RetornaMock();
        var clientMockB = fixture.ClientFactoryMock.AoCriarPara("modelo-fallback").RetornaMock();

        fixture.DownloaderMock.AoBaixar().Retorna([0x01, 0x02]);
        clientMockA.AoAnalisar().LancaExcecao(new LlmIndisponivelException("modelo-principal", 429, "Quota excedida"));
        clientMockB.AoAnalisar().Retorna(resultadoEsperado);

        // Act
        var resultado = await fixture.Service.AnalisarDiagramaAsync(Guid.NewGuid(), "arquivo.png", "https://bucket/arquivo.png", ".png");

        // Assert
        resultado.DeveSerSucesso(2);
        fixture.ClientFactoryMock.DeveTerCriadoPara("modelo-principal");
        fixture.ClientFactoryMock.DeveTerCriadoPara("modelo-fallback");
    }

    [Fact(DisplayName = "Deve fazer fallback para segundo modelo quando primeiro retorna 503")]
    [Trait("Infrastructure", "DiagramaAnaliseService")]
    public async Task AnalisarDiagramaAsync_DeveFazerFallback_QuandoPrimeiroModeloRetorna503()
    {
        // Arrange
        var fixture = new DiagramaAnaliseServiceTestFixture(maxTentativasPorModelo: 1, delaySegundos: 0);
        var resultadoEsperado = new ResultadoAnaliseDtoBuilder().Sucesso().Build();

        var clientMockA = fixture.ClientFactoryMock.AoCriarPara("modelo-principal").RetornaMock();
        var clientMockB = fixture.ClientFactoryMock.AoCriarPara("modelo-fallback").RetornaMock();

        fixture.DownloaderMock.AoBaixar().Retorna([0x01, 0x02]);
        clientMockA.AoAnalisar().LancaExcecao(new LlmIndisponivelException("modelo-principal", 503, "Serviço indisponível"));
        clientMockB.AoAnalisar().Retorna(resultadoEsperado);

        // Act
        var resultado = await fixture.Service.AnalisarDiagramaAsync(Guid.NewGuid(), "arquivo.png", "https://bucket/arquivo.png", ".png");

        // Assert
        resultado.DeveSerSucesso(2);
        fixture.ClientFactoryMock.DeveTerCriadoPara("modelo-principal");
        fixture.ClientFactoryMock.DeveTerCriadoPara("modelo-fallback");
    }

    [Fact(DisplayName = "Deve fazer fallback para segundo modelo quando primeiro esgota retries transitórios")]
    [Trait("Infrastructure", "DiagramaAnaliseService")]
    public async Task AnalisarDiagramaAsync_DeveFazerFallback_QuandoPrimeiroModeloEsgotaRetries()
    {
        // Arrange
        var fixture = new DiagramaAnaliseServiceTestFixture(maxTentativasPorModelo: 1, delaySegundos: 0);
        var resultadoEsperado = new ResultadoAnaliseDtoBuilder().Sucesso().Build();

        var clientMockA = fixture.ClientFactoryMock.AoCriarPara("modelo-principal").RetornaMock();
        var clientMockB = fixture.ClientFactoryMock.AoCriarPara("modelo-fallback").RetornaMock();

        fixture.DownloaderMock.AoBaixar().Retorna([0x01, 0x02]);
        clientMockA.AoAnalisar().LancaExcecao(new LlmTransientException("Timeout na LLM"));
        clientMockB.AoAnalisar().Retorna(resultadoEsperado);

        // Act
        var resultado = await fixture.Service.AnalisarDiagramaAsync(Guid.NewGuid(), "arquivo.png", "https://bucket/arquivo.png", ".png");

        // Assert
        resultado.Sucesso.ShouldBeTrue();
        fixture.ClientFactoryMock.DeveTerCriadoPara("modelo-principal");
        fixture.ClientFactoryMock.DeveTerCriadoPara("modelo-fallback");
    }

    [Fact(DisplayName = "Deve retornar falha permanente sem tentar fallback quando erro é permanente")]
    [Trait("Infrastructure", "DiagramaAnaliseService")]
    public async Task AnalisarDiagramaAsync_DeveRetornarFalhaPermanente_SemFallback()
    {
        // Arrange
        var fixture = new DiagramaAnaliseServiceTestFixture(maxTentativasPorModelo: 1, delaySegundos: 0);
        var clientMock = fixture.ClientFactoryMock.AoCriarPara("modelo-principal").RetornaMock();

        fixture.DownloaderMock.AoBaixar().Retorna([0x01, 0x02]);
        clientMock.AoAnalisar().LancaExcecao(new LlmPermanentException("Imagem inválida"));

        // Act
        var resultado = await fixture.Service.AnalisarDiagramaAsync(Guid.NewGuid(), "arquivo.png", "https://bucket/arquivo.png", ".png");

        // Assert
        resultado.DeveSerFalhaComOrigem(global::Shared.Constants.OrigemErroConstantes.Llm, 1);
        fixture.ClientFactoryMock.NaoDeveTerCriadoPara("modelo-fallback");
    }

    [Fact(DisplayName = "Deve retornar falha quando todos os modelos retornam 429")]
    [Trait("Infrastructure", "DiagramaAnaliseService")]
    public async Task AnalisarDiagramaAsync_DeveRetornarFalha_QuandoTodosModelosRetornam429()
    {
        // Arrange
        var fixture = new DiagramaAnaliseServiceTestFixture(maxTentativasPorModelo: 1, delaySegundos: 0);

        var clientMockA = fixture.ClientFactoryMock.AoCriarPara("modelo-principal").RetornaMock();
        var clientMockB = fixture.ClientFactoryMock.AoCriarPara("modelo-fallback").RetornaMock();

        fixture.DownloaderMock.AoBaixar().Retorna([0x01, 0x02]);
        clientMockA.AoAnalisar().LancaExcecao(new LlmIndisponivelException("modelo-principal", 429, "Quota excedida"));
        clientMockB.AoAnalisar().LancaExcecao(new LlmIndisponivelException("modelo-fallback", 429, "Quota excedida"));

        // Act
        var resultado = await fixture.Service.AnalisarDiagramaAsync(Guid.NewGuid(), "arquivo.png", "https://bucket/arquivo.png", ".png");

        // Assert
        resultado.DeveSerFalhaComOrigem(global::Shared.Constants.OrigemErroConstantes.Llm, 2);
    }

    [Fact(DisplayName = "Deve retornar sucesso no terceiro modelo após cascata de 429")]
    [Trait("Infrastructure", "DiagramaAnaliseService")]
    public async Task AnalisarDiagramaAsync_DeveRetornarSucessoNoTerceiroModelo_AposCascataDe429()
    {
        // Arrange
        var modelos = new List<string> { "modelo-a", "modelo-b", "modelo-c" };
        var fixture = new DiagramaAnaliseServiceTestFixture(maxTentativasPorModelo: 1, delaySegundos: 0, modelos: modelos);
        var resultadoEsperado = new ResultadoAnaliseDtoBuilder().Sucesso().Build();

        var clientMockA = fixture.ClientFactoryMock.AoCriarPara("modelo-a").RetornaMock();
        var clientMockB = fixture.ClientFactoryMock.AoCriarPara("modelo-b").RetornaMock();
        var clientMockC = fixture.ClientFactoryMock.AoCriarPara("modelo-c").RetornaMock();

        fixture.DownloaderMock.AoBaixar().Retorna([0x01, 0x02]);
        clientMockA.AoAnalisar().LancaExcecao(new LlmIndisponivelException("modelo-a", 429, "Quota excedida"));
        clientMockB.AoAnalisar().LancaExcecao(new LlmIndisponivelException("modelo-b", 429, "Quota excedida"));
        clientMockC.AoAnalisar().Retorna(resultadoEsperado);

        // Act
        var resultado = await fixture.Service.AnalisarDiagramaAsync(Guid.NewGuid(), "arquivo.png", "https://bucket/arquivo.png", ".png");

        // Assert
        resultado.DeveSerSucesso(3);
        fixture.ClientFactoryMock.DeveTerCriadoPara("modelo-a");
        fixture.ClientFactoryMock.DeveTerCriadoPara("modelo-b");
        fixture.ClientFactoryMock.DeveTerCriadoPara("modelo-c");
    }

    [Fact(DisplayName = "Deve retornar falha após esgotar tentativas transitórias em todos os modelos")]
    [Trait("Infrastructure", "DiagramaAnaliseService")]
    public async Task AnalisarDiagramaAsync_DeveRetornarFalha_QuandoLlmFalhaTransitoriaEmTodosModelos()
    {
        // Arrange
        var fixture = new DiagramaAnaliseServiceTestFixture(maxTentativasPorModelo: 1, delaySegundos: 0);

        var clientMockA = fixture.ClientFactoryMock.AoCriarPara("modelo-principal").RetornaMock();
        var clientMockB = fixture.ClientFactoryMock.AoCriarPara("modelo-fallback").RetornaMock();

        fixture.DownloaderMock.AoBaixar().Retorna([0x0A, 0x0B]);
        clientMockA.AoAnalisar().LancaExcecao(new LlmTransientException("Timeout na LLM"));
        clientMockB.AoAnalisar().LancaExcecao(new LlmTransientException("Timeout na LLM"));

        // Act
        var resultado = await fixture.Service.AnalisarDiagramaAsync(Guid.NewGuid(), "arquivo.png", "https://bucket/arquivo.png", ".png");

        // Assert
        resultado.DeveSerFalhaComOrigem(global::Shared.Constants.OrigemErroConstantes.Llm, resultado.TentativasRealizadas);
    }

    [Fact(DisplayName = "Deve retornar falha com origem armazenamento quando URL está vazia")]
    [Trait("Infrastructure", "DiagramaAnaliseService")]
    public async Task AnalisarDiagramaAsync_DeveRetornarFalha_QuandoUrlVazia()
    {
        // Arrange
        var fixture = new DiagramaAnaliseServiceTestFixture(maxTentativasPorModelo: 1, delaySegundos: 0);

        // Act
        var resultado = await fixture.Service.AnalisarDiagramaAsync(Guid.NewGuid(), "arquivo.png", "", ".png");

        // Assert
        resultado.DeveSerFalhaComOrigem(global::Shared.Constants.OrigemErroConstantes.Armazenamento, 0);
    }
}
