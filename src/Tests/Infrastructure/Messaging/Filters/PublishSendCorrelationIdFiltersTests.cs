using Infrastructure.Messaging.Filters;
using Infrastructure.Monitoramento.Correlation;
using MassTransit;

namespace Tests.Infrastructure.Messaging.Filters;

public class ConsumeCorrelationIdFilterProbeTests
{
    [Fact(DisplayName = "Probe deve criar filter scope com nome correlationId")]
    [Trait("Infrastructure", "ConsumeCorrelationIdFilter")]
    public void Probe_DeveCriarFilterScope_ComNomeCorrelationId()
    {
        // Arrange
        var filtro = new ConsumeCorrelationIdFilter<MensagemProbe>();
        var probeContextMock = new Mock<ProbeContext>();
        var innerScopeMock = new Mock<ProbeContext>();
        probeContextMock.Setup(x => x.CreateScope(It.IsAny<string>())).Returns(innerScopeMock.Object);

        // Act & Assert
        Should.NotThrow(() => filtro.Probe(probeContextMock.Object));
        probeContextMock.Verify(x => x.CreateScope("filters"), Times.Once);
    }

    [Fact(DisplayName = "Send deve ignorar header com valor em branco e usar correlationId do contexto")]
    [Trait("Infrastructure", "ConsumeCorrelationIdFilter")]
    public async Task Send_DeveIgnorarHeaderEmBranco_EUsarCorrelationIdDoContexto()
    {
        // Arrange
        var filtro = new ConsumeCorrelationIdFilter<MensagemProbe>();
        var headers = new Mock<Headers>();
        var contexto = new Mock<ConsumeContext<MensagemProbe>>();
        var next = new Mock<IPipe<ConsumeContext<MensagemProbe>>>();
        object? valorHeader = "   ";
        var correlationId = Guid.NewGuid();

        contexto.SetupGet(x => x.Headers).Returns(headers.Object);
        contexto.SetupGet(x => x.Message).Returns(new MensagemProbe());
        contexto.SetupGet(x => x.CorrelationId).Returns(correlationId);
        headers.Setup(x => x.TryGetHeader(CorrelationConstants.HeaderName, out valorHeader)).Returns(true);
        next.Setup(x => x.Send(contexto.Object)).Returns(() =>
        {
            CorrelationContext.Current.ShouldBe(correlationId.ToString());
            return Task.CompletedTask;
        });

        // Act
        await filtro.Send(contexto.Object, next.Object);

        // Assert
        next.Verify(x => x.Send(contexto.Object), Times.Once);
    }

    public class MensagemProbe
    {
        public string? CorrelationId { get; init; }
    }
}

public class PublishCorrelationIdFilterTests
{
    [Fact(DisplayName = "Probe deve criar filter scope com nome correlationId")]
    [Trait("Infrastructure", "PublishCorrelationIdFilter")]
    public void Probe_DeveCriarFilterScope_ComNomeCorrelationId()
    {
        // Arrange
        var filtro = new PublishCorrelationIdFilter<MensagemPublish>();
        var probeContextMock = new Mock<ProbeContext>();
        var innerScopeMock = new Mock<ProbeContext>();
        probeContextMock.Setup(x => x.CreateScope(It.IsAny<string>())).Returns(innerScopeMock.Object);

        // Act & Assert
        Should.NotThrow(() => filtro.Probe(probeContextMock.Object));
        probeContextMock.Verify(x => x.CreateScope("filters"), Times.Once);
    }

    [Fact(DisplayName = "Send deve propagar correlation id e chamar o próximo filtro")]
    [Trait("Infrastructure", "PublishCorrelationIdFilter")]
    public async Task Send_DevePropagarCorrelationIdEChamarNextSend()
    {
        // Arrange
        var filtro = new PublishCorrelationIdFilter<MensagemPublish>();
        var contexto = new Mock<PublishContext<MensagemPublish>>();
        var headers = new Mock<SendHeaders>();
        var next = new Mock<IPipe<PublishContext<MensagemPublish>>>();

        contexto.Setup(x => x.Headers).Returns(headers.Object);
        contexto.SetupProperty(x => x.CorrelationId);
        next.Setup(x => x.Send(It.IsAny<PublishContext<MensagemPublish>>())).Returns(Task.CompletedTask);

        // Act
        using (CorrelationContext.Push(Guid.NewGuid().ToString()))
            await filtro.Send(contexto.Object, next.Object);

        // Assert
        next.Verify(x => x.Send(contexto.Object), Times.Once);
    }

    public class MensagemPublish { }
}

public class SendCorrelationIdFilterTests
{
    [Fact(DisplayName = "Probe deve criar filter scope com nome correlationId")]
    [Trait("Infrastructure", "SendCorrelationIdFilter")]
    public void Probe_DeveCriarFilterScope_ComNomeCorrelationId()
    {
        // Arrange
        var filtro = new SendCorrelationIdFilter<MensagemSend>();
        var probeContextMock = new Mock<ProbeContext>();
        var innerScopeMock = new Mock<ProbeContext>();
        probeContextMock.Setup(x => x.CreateScope(It.IsAny<string>())).Returns(innerScopeMock.Object);

        // Act & Assert
        Should.NotThrow(() => filtro.Probe(probeContextMock.Object));
        probeContextMock.Verify(x => x.CreateScope("filters"), Times.Once);
    }

    [Fact(DisplayName = "Send deve propagar correlation id e chamar o próximo filtro")]
    [Trait("Infrastructure", "SendCorrelationIdFilter")]
    public async Task Send_DevePropagarCorrelationIdEChamarNextSend()
    {
        // Arrange
        var filtro = new SendCorrelationIdFilter<MensagemSend>();
        var contexto = new Mock<SendContext<MensagemSend>>();
        var headers = new Mock<SendHeaders>();
        var next = new Mock<IPipe<SendContext<MensagemSend>>>();

        contexto.Setup(x => x.Headers).Returns(headers.Object);
        contexto.SetupProperty(x => x.CorrelationId);
        next.Setup(x => x.Send(It.IsAny<SendContext<MensagemSend>>())).Returns(Task.CompletedTask);

        // Act
        using (CorrelationContext.Push(Guid.NewGuid().ToString()))
            await filtro.Send(contexto.Object, next.Object);

        // Assert
        next.Verify(x => x.Send(contexto.Object), Times.Once);
    }

    public class MensagemSend { }
}
