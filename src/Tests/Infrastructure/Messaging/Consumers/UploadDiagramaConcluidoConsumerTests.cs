namespace Tests.Infrastructure.Messaging.Consumers;

public class UploadDiagramaConcluidoConsumerTests : IDisposable
{
    private readonly UploadDiagramaConcluidoConsumerTestFixture _fixture = new();

    [Fact(DisplayName = "Deve processar mensagem de upload concluído sem lançar exceção")]
    [Trait("Infrastructure", "UploadDiagramaConcluidoConsumer")]
    public async Task Consume_DeveProcessarMensagem_QuandoDadosValidos()
    {
        // Arrange
        var mensagem = UploadDiagramaConcluidoConsumerTestFixture.CriarMensagemValida();
        _fixture.LlmServiceMock.AoAnalisar().Retorna(new ResultadoAnaliseDtoBuilder().Sucesso().Build());
        var consumeContext = UploadDiagramaConcluidoConsumerTestFixture.CriarConsumeContext(mensagem);

        // Act
        var acao = async () => await _fixture.Consumer.Consume(consumeContext.Object);

        // Assert
        await acao.ShouldNotThrowAsync();
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
