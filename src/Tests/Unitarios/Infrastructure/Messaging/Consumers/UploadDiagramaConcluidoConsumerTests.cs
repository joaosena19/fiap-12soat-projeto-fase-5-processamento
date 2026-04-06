using Microsoft.EntityFrameworkCore;

namespace Tests.Infrastructure.Messaging.Consumers;

public class UploadDiagramaConcluidoConsumerTests : IDisposable
{
    private readonly UploadDiagramaConcluidoConsumerTestFixture _fixture = new();

    [Fact(DisplayName = "Deve persistir processamento e publicar eventos quando mensagem é válida")]
    [Trait("Infrastructure", "UploadDiagramaConcluidoConsumer")]
    public async Task Consume_DeveProcessarMensagem_QuandoDadosValidos()
    {
        // Arrange
        var mensagem = UploadDiagramaConcluidoConsumerTestFixture.CriarMensagemValida();
        _fixture.LlmServiceMock.AoAnalisar().Retorna(new ResultadoAnaliseDtoBuilder().Sucesso().Build());
        var consumeContext = UploadDiagramaConcluidoConsumerTestFixture.CriarConsumeContext(mensagem);

        // Act
        await _fixture.Consumer.Consume(consumeContext.Object);

        var processamentoPersistido = await _fixture.Context.ProcessamentoDiagramas.SingleAsync(item => item.AnaliseDiagramaId == mensagem.AnaliseDiagramaId);

        // Assert
        processamentoPersistido.AnaliseDiagramaId.ShouldBe(mensagem.AnaliseDiagramaId);
        processamentoPersistido.StatusProcessamento.Valor.ShouldBe(StatusProcessamentoEnum.Concluido);
        processamentoPersistido.AnaliseResultado.ShouldNotBeNull();
        _fixture.PublisherMock.DeveTerPublicadoProcessamentoIniciado();
        _fixture.PublisherMock.DeveTerPublicadoDiagramaAnalisado();
    }

    [Fact(DisplayName = "Deve relançar exceção quando processamento falha durante consumo")]
    [Trait("Infrastructure", "UploadDiagramaConcluidoConsumer")]
    public async Task Consume_DeveRelancarExcecao_QuandoProcessamentoFalha()
    {
        // Arrange
        var mensagem = UploadDiagramaConcluidoConsumerTestFixture.CriarMensagemValida();
        _fixture.LlmServiceMock.AoAnalisar().LancaExcecao(new InvalidOperationException("falha de processamento"));
        var consumeContext = UploadDiagramaConcluidoConsumerTestFixture.CriarConsumeContext(mensagem);

        // Act & Assert
        await Should.ThrowAsync<InvalidOperationException>(() => _fixture.Consumer.Consume(consumeContext.Object));
    }

    public void Dispose() => _fixture.Dispose();
}
