using Application.Contracts.Messaging.Dtos;
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
        var correlationId = Guid.NewGuid().ToString();
        var processamento = new ProcessamentoDiagramaBuilder().EmProcessamento().Build();
        _fixture.CorrelationIdAccessorMock.AoObterCorrelationId(correlationId);

        // Act
        await _fixture.Publisher.PublicarProcessamentoIniciadoAsync(processamento, "diagrama.png", ".png");

        // Assert
        var mensagem = _fixture.PublishEndpointMock.ObterMensagemPublicada<ProcessamentoDiagramaIniciadoDto>();
        mensagem.DeveConterDadosInicializacao(correlationId, processamento, "diagrama.png", ".png");
    }

    [Fact(DisplayName = "Deve publicar processamento iniciado com data início nula usando fallback UTC")]
    [Trait("Infrastructure", "ProcessamentoDiagramaMessagePublisher")]
    public async Task PublicarProcessamentoIniciadoAsync_DeveUsarUtcNow_QuandoDataInicioNula()
    {
        // Arrange — processamento sem IniciarProcessamento, DataInicioProcessamento = null
        var correlationId = Guid.NewGuid().ToString();
        var processamento = new ProcessamentoDiagramaBuilder().Build();
        _fixture.CorrelationIdAccessorMock.AoObterCorrelationId(correlationId);
        var inicioMinimo = DateTimeOffset.UtcNow;

        // Act
        await _fixture.Publisher.PublicarProcessamentoIniciadoAsync(processamento, "diagrama.png", ".png");
        var inicioMaximo = DateTimeOffset.UtcNow;

        // Assert
        var mensagem = _fixture.PublishEndpointMock.ObterMensagemPublicada<ProcessamentoDiagramaIniciadoDto>();
        mensagem.CorrelationId.ShouldBe(correlationId);
        mensagem.AnaliseDiagramaId.ShouldBe(processamento.AnaliseDiagramaId);
        mensagem.DataInicio.ShouldBeInRange(inicioMinimo, inicioMaximo);
    }

    [Fact(DisplayName = "Deve publicar processamento analisado")]
    [Trait("Infrastructure", "ProcessamentoDiagramaMessagePublisher")]
    public async Task PublicarDiagramaAnalisadoAsync_DevePublicar_QuandoProcessamentoConcluido()
    {
        // Arrange
        var correlationId = Guid.NewGuid().ToString();
        var processamento = new ProcessamentoDiagramaBuilder().Concluido().Build();
        _fixture.CorrelationIdAccessorMock.AoObterCorrelationId(correlationId);

        // Act
        await _fixture.Publisher.PublicarDiagramaAnalisadoAsync(processamento);

        // Assert
        var mensagem = _fixture.PublishEndpointMock.ObterMensagemPublicada<ProcessamentoDiagramaAnalisadoDto>();
        mensagem.DeveConterDadosConclusao(correlationId, processamento);
    }

    [Fact(DisplayName = "Deve publicar processamento analisado com analise nula usando listas vazias")]
    [Trait("Infrastructure", "ProcessamentoDiagramaMessagePublisher")]
    public async Task PublicarDiagramaAnalisadoAsync_DeveUsarListasVazias_QuandoAnaliseResultadoNulo()
    {
        // Arrange — processamento sem ConcluirProcessamento, AnaliseResultado = null e DataConclusaoProcessamento = null
        var correlationId = Guid.NewGuid().ToString();
        var processamento = new ProcessamentoDiagramaBuilder().Build();
        _fixture.CorrelationIdAccessorMock.AoObterCorrelationId(correlationId);
        var conclusaoMinima = DateTimeOffset.UtcNow;

        // Act
        await _fixture.Publisher.PublicarDiagramaAnalisadoAsync(processamento);
        var conclusaoMaxima = DateTimeOffset.UtcNow;

        // Assert
        var mensagem = _fixture.PublishEndpointMock.ObterMensagemPublicada<ProcessamentoDiagramaAnalisadoDto>();
        mensagem.CorrelationId.ShouldBe(correlationId);
        mensagem.AnaliseDiagramaId.ShouldBe(processamento.AnaliseDiagramaId);
        mensagem.DescricaoAnalise.ShouldBe(string.Empty);
        mensagem.ComponentesIdentificados.ShouldBeEmpty();
        mensagem.RiscosArquiteturais.ShouldBeEmpty();
        mensagem.RecomendacoesBasicas.ShouldBeEmpty();
        mensagem.DataConclusao.ShouldBeInRange(conclusaoMinima, conclusaoMaxima);
    }

    [Fact(DisplayName = "Deve publicar processamento com erro")]
    [Trait("Infrastructure", "ProcessamentoDiagramaMessagePublisher")]
    public async Task PublicarProcessamentoErroAsync_DevePublicar_QuandoProcessamentoComFalha()
    {
        // Arrange
        var correlationId = Guid.NewGuid().ToString();
        var processamento = new ProcessamentoDiagramaBuilder().ComFalha(2).Build();
        _fixture.CorrelationIdAccessorMock.AoObterCorrelationId(correlationId);

        // Act
        await _fixture.Publisher.PublicarProcessamentoErroAsync(processamento, "falha no processamento");

        // Assert
        var mensagem = _fixture.PublishEndpointMock.ObterMensagemPublicada<ProcessamentoDiagramaErroDto>();
        mensagem.DeveConterDadosErro(correlationId, processamento, "falha no processamento");
    }

    [Fact(DisplayName = "Deve publicar processamento com erro com data conclusão nula usando fallback UTC")]
    [Trait("Infrastructure", "ProcessamentoDiagramaMessagePublisher")]
    public async Task PublicarProcessamentoErroAsync_DeveUsarUtcNow_QuandoDataConclusaoNula()
    {
        // Arrange — processamento sem MarcarConclusaoProcessamento, DataConclusaoProcessamento = null
        var correlationId = Guid.NewGuid().ToString();
        var processamento = new ProcessamentoDiagramaBuilder().Build();
        _fixture.CorrelationIdAccessorMock.AoObterCorrelationId(correlationId);
        var erroMinimo = DateTimeOffset.UtcNow;

        // Act
        await _fixture.Publisher.PublicarProcessamentoErroAsync(processamento, "motivo qualquer");
        var erroMaximo = DateTimeOffset.UtcNow;

        // Assert
        var mensagem = _fixture.PublishEndpointMock.ObterMensagemPublicada<ProcessamentoDiagramaErroDto>();
        mensagem.CorrelationId.ShouldBe(correlationId);
        mensagem.AnaliseDiagramaId.ShouldBe(processamento.AnaliseDiagramaId);
        mensagem.Motivo.ShouldBe("motivo qualquer");
        mensagem.TentativasRealizadas.ShouldBe(processamento.TentativasProcessamento.Valor);
        mensagem.DataErro.ShouldBeInRange(erroMinimo, erroMaximo);
    }
}
