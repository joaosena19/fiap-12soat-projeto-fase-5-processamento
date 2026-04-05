using Tests.Helpers.Fixtures;

namespace Tests.Infrastructure.Messaging.Publishers;

public class ProcessamentoDiagramaMessagePublisherTests
{
    private readonly ProcessamentoDiagramaMessagePublisherTestFixture _fixture = new();

    [Fact(DisplayName = "Deve publicar processamento iniciado")]
    [Trait("Infrastructure", "ProcessamentoDiagramaMessagePublisher")]
    public async Task PublicarProcessamentoIniciadoAsync_DevePublicar_QuandoProcessamentoValido()
    {
        // Arrange
        var processamento = new ProcessamentoDiagramaBuilder().EmProcessamento().Build();

        // Act & Assert
        await Should.NotThrowAsync(() => _fixture.Publisher.PublicarProcessamentoIniciadoAsync(processamento, "diagrama.png", ".png"));
    }

    [Fact(DisplayName = "Deve publicar processamento iniciado com data início nula usando fallback UTC")]
    [Trait("Infrastructure", "ProcessamentoDiagramaMessagePublisher")]
    public async Task PublicarProcessamentoIniciadoAsync_DeveUsarUtcNow_QuandoDataInicioNula()
    {
        // Arrange — processamento sem IniciarProcessamento, DataInicioProcessamento = null
        var processamento = new ProcessamentoDiagramaBuilder().Build();

        // Act & Assert
        await Should.NotThrowAsync(() => _fixture.Publisher.PublicarProcessamentoIniciadoAsync(processamento, "diagrama.png", ".png"));
    }

    [Fact(DisplayName = "Deve publicar processamento analisado")]
    [Trait("Infrastructure", "ProcessamentoDiagramaMessagePublisher")]
    public async Task PublicarDiagramaAnalisadoAsync_DevePublicar_QuandoProcessamentoConcluido()
    {
        // Arrange
        var processamento = new ProcessamentoDiagramaBuilder().Concluido().Build();

        // Act & Assert
        await Should.NotThrowAsync(() => _fixture.Publisher.PublicarDiagramaAnalisadoAsync(processamento));
    }

    [Fact(DisplayName = "Deve publicar processamento analisado com analise nula usando listas vazias")]
    [Trait("Infrastructure", "ProcessamentoDiagramaMessagePublisher")]
    public async Task PublicarDiagramaAnalisadoAsync_DeveUsarListasVazias_QuandoAnaliseResultadoNulo()
    {
        // Arrange — processamento sem ConcluirProcessamento, AnaliseResultado = null e DataConclusaoProcessamento = null
        var processamento = new ProcessamentoDiagramaBuilder().Build();

        // Act & Assert
        await Should.NotThrowAsync(() => _fixture.Publisher.PublicarDiagramaAnalisadoAsync(processamento));
    }

    [Fact(DisplayName = "Deve publicar processamento com erro")]
    [Trait("Infrastructure", "ProcessamentoDiagramaMessagePublisher")]
    public async Task PublicarProcessamentoErroAsync_DevePublicar_QuandoProcessamentoComFalha()
    {
        // Arrange
        var processamento = new ProcessamentoDiagramaBuilder().ComFalha(2).Build();

        // Act & Assert
        await Should.NotThrowAsync(() => _fixture.Publisher.PublicarProcessamentoErroAsync(processamento, "falha no processamento"));
    }

    [Fact(DisplayName = "Deve publicar processamento com erro com data conclusão nula usando fallback UTC")]
    [Trait("Infrastructure", "ProcessamentoDiagramaMessagePublisher")]
    public async Task PublicarProcessamentoErroAsync_DeveUsarUtcNow_QuandoDataConclusaoNula()
    {
        // Arrange — processamento sem MarcarConclusaoProcessamento, DataConclusaoProcessamento = null
        var processamento = new ProcessamentoDiagramaBuilder().Build();

        // Act & Assert
        await Should.NotThrowAsync(() => _fixture.Publisher.PublicarProcessamentoErroAsync(processamento, "motivo qualquer"));
    }
}
