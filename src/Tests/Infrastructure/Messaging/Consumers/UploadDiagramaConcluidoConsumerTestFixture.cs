using Application.Contracts.Messaging.Dtos;
using Infrastructure.Messaging.Consumers;
using MassTransit;
using Tests.Helpers.Fixtures;

namespace Tests.Infrastructure.Messaging.Consumers;

public class UploadDiagramaConcluidoConsumerTestFixture : IDisposable
{
    private readonly AppDbContextTestFixture _dbFixture;
    public Mock<IDiagramaAnaliseService> LlmServiceMock { get; } = new();
    public Mock<IProcessamentoDiagramaMessagePublisher> PublisherMock { get; } = new();
    public UploadDiagramaConcluidoConsumer Consumer { get; }

    public UploadDiagramaConcluidoConsumerTestFixture()
    {
        _dbFixture = new AppDbContextTestFixture();
        PublisherMock.AoPublicarNaoFazNada();
        Consumer = new UploadDiagramaConcluidoConsumer(_dbFixture.Context, LlmServiceMock.Object, PublisherMock.Object, new LoggerFactory());
    }

    public static Mock<ConsumeContext<UploadDiagramaConcluidoDto>> CriarConsumeContext(UploadDiagramaConcluidoDto mensagem)
    {
        var consumeContext = new Mock<ConsumeContext<UploadDiagramaConcluidoDto>>();
        consumeContext.SetupGet(x => x.Message).Returns(mensagem);
        consumeContext.SetupGet(x => x.MessageId).Returns(Guid.NewGuid());
        return consumeContext;
    }

    public static UploadDiagramaConcluidoDto CriarMensagemValida() => new UploadDiagramaConcluidoDtoBuilder().Build();

    public void Dispose() => _dbFixture.Dispose();
}
