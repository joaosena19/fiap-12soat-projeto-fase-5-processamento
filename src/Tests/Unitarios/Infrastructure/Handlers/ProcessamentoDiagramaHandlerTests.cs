using Domain.ProcessamentoDiagrama.Aggregates;
using Tests.Helpers.Fixtures;

namespace Tests.Infrastructure.Handlers;

public class ProcessamentoDiagramaHandlerTests
{
    private readonly ProcessamentoDiagramaHandlerTestFixture _fixture;

    public ProcessamentoDiagramaHandlerTests()
    {
        _fixture = new ProcessamentoDiagramaHandlerTestFixture();
    }

    [Fact(DisplayName = "Deve criar registro inicial e processar quando nao existir processamento")]
    [Trait("Handler", "ProcessamentoDiagramaHandler")]
    public async Task IniciarProcessamentoAsync_DeveCriarRegistroEProcessar_QuandoNaoExiste()
    {
        // Arrange
        var dto = new ProcessarDiagramaDtoBuilder().Build();
        var processamento = ProcessamentoDiagramaAggregate.Criar(dto.AnaliseDiagramaId);
        var resultado = new ResultadoAnaliseDtoBuilder().Sucesso().Build();

        _fixture.GatewayMock.AoObterPorAnaliseDiagramaIdSequencial(dto.AnaliseDiagramaId)
            .RetornaPrimeiro(null, processamento);

        _fixture.LlmServiceMock.AoAnalisar().Retorna(resultado);

        // Act
        await _fixture.IniciarProcessamentoAsync(dto);

        // Assert
        _fixture.GatewayMock.DeveTerSalvo(3);
        _fixture.MessagePublisherMock.DeveTerPublicadoProcessamentoIniciado();
        _fixture.MessagePublisherMock.DeveTerPublicadoDiagramaAnalisado();
    }

    [Fact(DisplayName = "Deve ignorar mensagem duplicada quando ja estiver em processamento")]
    [Trait("Handler", "ProcessamentoDiagramaHandler")]
    public async Task IniciarProcessamentoAsync_DeveIgnorar_QuandoJaEmProcessamento()
    {
        // Arrange
        var dto = new ProcessarDiagramaDtoBuilder().Build();
        var processamento = new ProcessamentoDiagramaBuilder().ComAnaliseDiagramaId(dto.AnaliseDiagramaId).EmProcessamento().Build();

        _fixture.GatewayMock.AoObterPorAnaliseDiagramaId(dto.AnaliseDiagramaId).Retorna(processamento);

        // Act
        await _fixture.IniciarProcessamentoAsync(dto);

        // Assert
        _fixture.LlmServiceMock.NaoDeveTerAnalisado();
        _fixture.GatewayMock.NaoDeveTerSalvo();
        _fixture.LoggerMock.DeveTerLogadoWarning();
    }

    [Fact(DisplayName = "Deve ignorar mensagem duplicada quando ja estiver concluido")]
    [Trait("Handler", "ProcessamentoDiagramaHandler")]
    public async Task IniciarProcessamentoAsync_DeveIgnorar_QuandoJaConcluido()
    {
        // Arrange
        var dto = new ProcessarDiagramaDtoBuilder().Build();
        var processamento = new ProcessamentoDiagramaBuilder().ComAnaliseDiagramaId(dto.AnaliseDiagramaId).Concluido().Build();

        _fixture.GatewayMock.AoObterPorAnaliseDiagramaId(dto.AnaliseDiagramaId).Retorna(processamento);

        // Act
        await _fixture.IniciarProcessamentoAsync(dto);

        // Assert
        _fixture.LlmServiceMock.NaoDeveTerAnalisado();
        _fixture.GatewayMock.NaoDeveTerSalvo();
        _fixture.LoggerMock.DeveTerLogadoWarning();
    }

    [Fact(DisplayName = "Deve reprocessar quando status atual for falha")]
    [Trait("Handler", "ProcessamentoDiagramaHandler")]
    public async Task IniciarProcessamentoAsync_DeveReprocessar_QuandoEstadoFalha()
    {
        // Arrange
        var dto = new ProcessarDiagramaDtoBuilder().Build();
        var processamento = new ProcessamentoDiagramaBuilder().ComAnaliseDiagramaId(dto.AnaliseDiagramaId).ComFalha(2).Build();
        var resultado = new ResultadoAnaliseDtoBuilder().Sucesso().Build();

        _fixture.GatewayMock.AoObterPorAnaliseDiagramaId(dto.AnaliseDiagramaId).Retorna(processamento);
        _fixture.LlmServiceMock.AoAnalisar().Retorna(resultado);

        // Act
        await _fixture.IniciarProcessamentoAsync(dto);

        // Assert
        _fixture.GatewayMock.DeveTerSalvo(2);
        _fixture.LlmServiceMock.DeveTerAnalisado();
        _fixture.MessagePublisherMock.DeveTerPublicadoDiagramaAnalisado();
    }
}
