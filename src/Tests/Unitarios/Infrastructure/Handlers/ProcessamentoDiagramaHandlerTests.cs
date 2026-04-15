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
        _fixture.LoggerMock.DeveTerLogadoInformation();
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
        _fixture.LoggerMock.DeveTerLogadoInformation();
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

    [Fact(DisplayName = "Deve ignorar mensagem quando processamento ja foi rejeitado")]
    [Trait("Handler", "ProcessamentoDiagramaHandler")]
    public async Task IniciarProcessamentoAsync_DeveIgnorar_QuandoStatusRejeitado()
    {
        // Arrange
        var dto = new ProcessarDiagramaDtoBuilder().Build();
        var processamento = new ProcessamentoDiagramaBuilder().ComAnaliseDiagramaId(dto.AnaliseDiagramaId).Rejeitado().Build();

        _fixture.GatewayMock.AoObterPorAnaliseDiagramaId(dto.AnaliseDiagramaId).Retorna(processamento);

        // Act
        await _fixture.IniciarProcessamentoAsync(dto);

        // Assert
        _fixture.LlmServiceMock.NaoDeveTerAnalisado();
        _fixture.GatewayMock.NaoDeveTerSalvo();
        _fixture.LoggerMock.DeveTerLogadoInformation();
    }

    [Fact(DisplayName = "Deve ignorar mensagem quando LocalizacaoUrl estiver vazia e nao existir processamento anterior")]
    [Trait("Handler", "ProcessamentoDiagramaHandler")]
    public async Task IniciarProcessamentoAsync_DeveIgnorar_QuandoLocalizacaoUrlVaziaENaoExisteProcessamento()
    {
        // Arrange
        var dto = new ProcessarDiagramaDtoBuilder().SemLocalizacaoUrl().Build();

        // Act
        await _fixture.IniciarProcessamentoAsync(dto);

        // Assert
        _fixture.LlmServiceMock.NaoDeveTerAnalisado();
        _fixture.GatewayMock.NaoDeveTerSalvo();
        _fixture.MessagePublisherMock.NaoDeveTerPublicadoProcessamentoErro();
        _fixture.LoggerMock.DeveTerLogadoWarning();
    }

    [Fact(DisplayName = "Deve recuperar LocalizacaoUrl do banco e processar quando retry com url vazia e dados de origem existentes")]
    [Trait("Handler", "ProcessamentoDiagramaHandler")]
    public async Task IniciarProcessamentoAsync_DeveRecuperarUrlDoBanco_QuandoRetryComUrlVaziaEDadosOrigemExistem()
    {
        // Arrange
        var dto = new ProcessarDiagramaDtoBuilder().SemLocalizacaoUrl().Build();
        var processamento = new ProcessamentoDiagramaBuilder()
            .ComAnaliseDiagramaId(dto.AnaliseDiagramaId)
            .ComDadosOrigem()
            .ComFalha(2)
            .Build();
        var resultado = new ResultadoAnaliseDtoBuilder().Sucesso().Build();

        _fixture.GatewayMock.AoObterPorAnaliseDiagramaId(dto.AnaliseDiagramaId).Retorna(processamento);
        _fixture.LlmServiceMock.AoAnalisar().Retorna(resultado);

        // Act
        await _fixture.IniciarProcessamentoAsync(dto);

        // Assert
        _fixture.LlmServiceMock.DeveTerAnalisado();
        _fixture.GatewayMock.DeveTerSalvo(2);
        _fixture.LoggerMock.DeveTerLogadoWarning();
    }

    [Fact(DisplayName = "Deve publicar erro quando retry com url vazia e sem dados de origem no banco")]
    [Trait("Handler", "ProcessamentoDiagramaHandler")]
    public async Task IniciarProcessamentoAsync_DevePublicarErro_QuandoRetryComUrlVaziaESemDadosOrigem()
    {
        // Arrange
        var dto = new ProcessarDiagramaDtoBuilder().SemLocalizacaoUrl().Build();
        var processamento = new ProcessamentoDiagramaBuilder()
            .ComAnaliseDiagramaId(dto.AnaliseDiagramaId)
            .ComFalha(2)
            .Build();

        _fixture.GatewayMock.AoObterPorAnaliseDiagramaId(dto.AnaliseDiagramaId).Retorna(processamento);

        // Act
        await _fixture.IniciarProcessamentoAsync(dto);

        // Assert
        _fixture.LlmServiceMock.NaoDeveTerAnalisado();
        _fixture.GatewayMock.NaoDeveTerSalvo();
        _fixture.MessagePublisherMock.DeveTerPublicadoProcessamentoErro();
        _fixture.LoggerMock.DeveTerLogadoWarning();
    }

    [Fact(DisplayName = "Deve persistir dados de origem quando criar registro inicial")]
    [Trait("Handler", "ProcessamentoDiagramaHandler")]
    public async Task IniciarProcessamentoAsync_DevePersistirDadosOrigem_QuandoCriarRegistroInicial()
    {
        // Arrange
        var dto = new ProcessarDiagramaDtoBuilder().Build();
        ProcessamentoDiagramaAggregate? processamentoSalvo = null;

        _fixture.GatewayMock.AoObterPorAnaliseDiagramaIdSequencial(dto.AnaliseDiagramaId)
            .RetornaPrimeiro(null, () => processamentoSalvo!);

        _fixture.GatewayMock.AoSalvar().ComCallback(p => processamentoSalvo = p);
        _fixture.LlmServiceMock.AoAnalisar().Retorna(new ResultadoAnaliseDtoBuilder().Sucesso().Build());

        // Act
        await _fixture.IniciarProcessamentoAsync(dto);

        // Assert
        processamentoSalvo.ShouldNotBeNull();
        processamentoSalvo.DeveConterDadosOrigem();
        processamentoSalvo.DadosOrigem!.LocalizacaoUrl.Valor.ShouldBe(dto.LocalizacaoUrl);
        processamentoSalvo.DadosOrigem.NomeFisico.Valor.ShouldBe(dto.NomeFisico);
    }
}
