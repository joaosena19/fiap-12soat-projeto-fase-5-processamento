using Tests.Helpers.Fixtures;

namespace Tests.Features.ProcessamentoDiagrama.ProcessarDiagrama;

public class ProcessarDiagramaUseCaseTests
{
    private readonly ProcessarDiagramaUseCaseTestFixture _fixture;

    public ProcessarDiagramaUseCaseTests()
    {
        _fixture = new ProcessarDiagramaUseCaseTestFixture();
    }

    [Fact(DisplayName = "Deve concluir processamento quando llm retorna sucesso")]
    [Trait("UseCase", "ProcessarDiagrama")]
    public async Task ExecutarAsync_DeveConcluirProcessamento_QuandoLlmRetornaSucesso()
    {
        // Arrange
        var dto = new ProcessarDiagramaDtoBuilder().Build();
        var processamento = new ProcessamentoDiagramaBuilder().ComAnaliseDiagramaId(dto.AnaliseDiagramaId).Build();
        var resultado = new ResultadoAnaliseDtoBuilder().Sucesso().ComTentativas(2).Build();

        _fixture.GatewayMock.AoObterPorAnaliseDiagramaId(dto.AnaliseDiagramaId).Retorna(processamento);
        _fixture.LlmServiceMock.AoAnalisar().Retorna(resultado);

        // Act
        await _fixture.ExecutarAsync(dto);

        // Assert
        _fixture.GatewayMock.DeveTerSalvo(2);
        _fixture.MessagePublisherMock.DeveTerPublicadoProcessamentoIniciado();
        _fixture.MessagePublisherMock.DeveTerPublicadoDiagramaAnalisado();
        _fixture.MessagePublisherMock.NaoDeveTerPublicadoProcessamentoErro();
        _fixture.MetricsMock.DeveTerRegistradoProcessamentoIniciado();
        _fixture.MetricsMock.DeveTerRegistradoProcessamentoConcluido();
        _fixture.MetricsMock.NaoDeveTerRegistradoProcessamentoFalha();
    }

    [Fact(DisplayName = "Deve registrar falha quando llm retorna erro")]
    [Trait("UseCase", "ProcessarDiagrama")]
    public async Task ExecutarAsync_DeveRegistrarFalha_QuandoLlmRetornaErro()
    {
        // Arrange
        var dto = new ProcessarDiagramaDtoBuilder().Build();
        var processamento = new ProcessamentoDiagramaBuilder().ComAnaliseDiagramaId(dto.AnaliseDiagramaId).Build();
        var resultado = new ResultadoAnaliseDtoBuilder().ComFalha("Erro na analise", 3).Build();

        _fixture.GatewayMock.AoObterPorAnaliseDiagramaId(dto.AnaliseDiagramaId).Retorna(processamento);
        _fixture.LlmServiceMock.AoAnalisar().Retorna(resultado);

        // Act
        await _fixture.ExecutarAsync(dto);

        // Assert
        _fixture.GatewayMock.DeveTerSalvo(2);
        _fixture.MessagePublisherMock.DeveTerPublicadoProcessamentoIniciado();
        _fixture.MessagePublisherMock.DeveTerPublicadoProcessamentoErroSemRejeicao();
        _fixture.MessagePublisherMock.NaoDeveTerPublicadoDiagramaAnalisado();
        _fixture.MetricsMock.DeveTerRegistradoProcessamentoIniciado();
        _fixture.MetricsMock.DeveTerRegistradoProcessamentoFalha();
        _fixture.MetricsMock.NaoDeveTerRegistradoProcessamentoConcluido();
        _fixture.LoggerMock.DeveTerLogadoError();
    }

    [Fact(DisplayName = "Deve registrar rejeicao com warning quando imagem nao eh diagrama")]
    [Trait("UseCase", "ProcessarDiagrama")]
    public async Task ExecutarAsync_DeveRegistrarRejeicao_QuandoImagemNaoEhDiagrama()
    {
        // Arrange
        var dto = new ProcessarDiagramaDtoBuilder().Build();
        var processamento = new ProcessamentoDiagramaBuilder().ComAnaliseDiagramaId(dto.AnaliseDiagramaId).Build();
        var resultado = new ResultadoAnaliseDtoBuilder().ComRejeicao("A imagem não é diagrama").Build();

        _fixture.GatewayMock.AoObterPorAnaliseDiagramaId(dto.AnaliseDiagramaId).Retorna(processamento);
        _fixture.LlmServiceMock.AoAnalisar().Retorna(resultado);

        // Act
        await _fixture.ExecutarAsync(dto);

        // Assert
        _fixture.GatewayMock.DeveTerSalvo(2);
        _fixture.MessagePublisherMock.DeveTerPublicadoProcessamentoIniciado();
        _fixture.MessagePublisherMock.DeveTerPublicadoProcessamentoErroComRejeicao();
        _fixture.MessagePublisherMock.NaoDeveTerPublicadoDiagramaAnalisado();
        _fixture.MetricsMock.DeveTerRegistradoProcessamentoIniciado();
        _fixture.MetricsMock.DeveTerRegistradoProcessamentoFalha();
        _fixture.MetricsMock.NaoDeveTerRegistradoProcessamentoConcluido();
        _fixture.LoggerMock.DeveTerLogadoWarning();
        _fixture.LoggerMock.NaoDeveTerLogadoError();
    }

    [Fact(DisplayName = "Deve usar motivo padrão quando llm retorna falha sem motivo")]
    [Trait("UseCase", "ProcessarDiagrama")]
    public async Task ExecutarAsync_DeveUsarMotivoPadrao_QuandoLlmRetornaFalhaSemMotivo()
    {
        // Arrange
        var dto = new ProcessarDiagramaDtoBuilder().Build();
        var processamento = new ProcessamentoDiagramaBuilder().ComAnaliseDiagramaId(dto.AnaliseDiagramaId).Build();
        var resultado = new ResultadoAnaliseDtoBuilder().ComFalhaSemMotivo(2).Build();

        _fixture.GatewayMock.AoObterPorAnaliseDiagramaId(dto.AnaliseDiagramaId).Retorna(processamento);
        _fixture.LlmServiceMock.AoAnalisar().Retorna(resultado);

        // Act
        await _fixture.ExecutarAsync(dto);

        // Assert
        _fixture.MessagePublisherMock.DeveTerPublicadoProcessamentoErroComMensagem("Erro desconhecido na análise do diagrama");
        _fixture.MetricsMock.DeveTerRegistradoProcessamentoFalha();
    }

    [Fact(DisplayName = "Deve lancar excecao quando processamento nao encontrado")]
    [Trait("UseCase", "ProcessarDiagrama")]
    public async Task ExecutarAsync_DeveLancarExcecao_QuandoProcessamentoNaoEncontrado()
    {
        // Arrange
        var dto = new ProcessarDiagramaDtoBuilder().Build();
        _fixture.GatewayMock.AoObterPorAnaliseDiagramaId(dto.AnaliseDiagramaId).NaoRetornaNada();

        // Act
        var excecao = await Should.ThrowAsync<DomainException>(async () => await _fixture.ExecutarAsync(dto));

        // Assert
        excecao.ErrorType.ShouldBe(ErrorType.ResourceNotFound);
        _fixture.GatewayMock.NaoDeveTerSalvo();
        _fixture.LlmServiceMock.NaoDeveTerAnalisado();
        _fixture.MessagePublisherMock.NaoDeveTerPublicadoProcessamentoIniciado();
        _fixture.LoggerMock.DeveTerLogadoErrorComException();
    }

    [Fact(DisplayName = "Deve propagar excecao quando llm lanca erro inesperado")]
    [Trait("UseCase", "ProcessarDiagrama")]
    public async Task ExecutarAsync_DevePropagarExcecao_QuandoErroInesperado()
    {
        // Arrange
        var dto = new ProcessarDiagramaDtoBuilder().Build();
        var processamento = new ProcessamentoDiagramaBuilder().ComAnaliseDiagramaId(dto.AnaliseDiagramaId).Build();

        _fixture.GatewayMock.AoObterPorAnaliseDiagramaId(dto.AnaliseDiagramaId).Retorna(processamento);
        _fixture.LlmServiceMock.AoAnalisar().LancaExcecao(new InvalidOperationException("Erro inesperado"));

        // Act
        await Should.ThrowAsync<InvalidOperationException>(async () => await _fixture.ExecutarAsync(dto));

        // Assert
        _fixture.LoggerMock.DeveTerLogadoErrorComException();
    }
}
