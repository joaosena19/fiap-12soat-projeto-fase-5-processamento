using Tests.Helpers.Fixtures;
using Infrastructure.LLM;

namespace Tests.Infrastructure.LLM;

public class DiagramaAnaliseServiceTests
{
    [Fact(DisplayName = "Deve retornar sucesso quando análise da LLM é concluída")]
    [Trait("Infrastructure", "DiagramaAnaliseService")]
    public async Task AnalisarDiagramaAsync_DeveRetornarSucesso_QuandoAnaliseConcluida()
    {
        // Arrange
        var fixture = new DiagramaAnaliseServiceTestFixture(maxTentativas: 1, delaySegundos: 0);
        var resultadoEsperado = new ResultadoAnaliseDtoBuilder().Sucesso().Build();

        fixture.DownloaderMock.AoBaixar().Retorna([0x01, 0x02]);
        fixture.ClienteLlmMock.AoAnalisar().Retorna(resultadoEsperado);

        // Act
        var resultado = await fixture.Service.AnalisarDiagramaAsync(Guid.NewGuid(), "arquivo.png", "https://bucket/arquivo.png", ".png");

        // Assert
        resultado.Sucesso.ShouldBeTrue();
        resultado.TentativasRealizadas.ShouldBe(1);
    }

    [Fact(DisplayName = "Deve retornar falha quando download do arquivo gera exceção")]
    [Trait("Infrastructure", "DiagramaAnaliseService")]
    public async Task AnalisarDiagramaAsync_DeveRetornarFalha_QuandoDownloadFalha()
    {
        // Arrange
        var fixture = new DiagramaAnaliseServiceTestFixture(maxTentativas: 1, delaySegundos: 0);

        fixture.DownloaderMock.AoBaixar().LancaExcecao(new InvalidOperationException("Falha no download"));

        // Act
        var resultado = await fixture.Service.AnalisarDiagramaAsync(Guid.NewGuid(), "arquivo.png", "https://bucket/arquivo.png", ".png");

        // Assert
        resultado.Sucesso.ShouldBeFalse();
        resultado.MotivoErro.ShouldNotBeNull();
        resultado.MotivoErro.ShouldContain("Falha no download");
        resultado.TentativasRealizadas.ShouldBe(0);
    }

    [Fact(DisplayName = "Deve retornar falha após esgotar tentativas em erro transitório da LLM")]
    [Trait("Infrastructure", "DiagramaAnaliseService")]
    public async Task AnalisarDiagramaAsync_DeveRetornarFalha_QuandoLlmFalhaAposRetries()
    {
        // Arrange
        var fixture = new DiagramaAnaliseServiceTestFixture(maxTentativas: 2, delaySegundos: 0);

        fixture.DownloaderMock.AoBaixar().Retorna([0x0A, 0x0B]);
        fixture.ClienteLlmMock.AoAnalisar().LancaExcecao(new LlmTransientException("Timeout na LLM"));

        // Act
        var resultado = await fixture.Service.AnalisarDiagramaAsync(Guid.NewGuid(), "arquivo.png", "https://bucket/arquivo.png", ".png");

        // Assert
        resultado.Sucesso.ShouldBeFalse();
        resultado.MotivoErro.ShouldNotBeNull();
        resultado.MotivoErro.ShouldContain("Timeout na LLM");
        resultado.TentativasRealizadas.ShouldBe(3);
    }
}
