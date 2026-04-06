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

        // Act & Assert
        ProbeContextAssertionExtensions.DeveResponderAoProbeComScopeFilters(filtro.Probe);
    }

    [Fact(DisplayName = "Send deve ignorar header com valor em branco e usar correlationId do contexto")]
    [Trait("Infrastructure", "ConsumeCorrelationIdFilter")]
    public async Task Send_DeveIgnorarHeaderEmBranco_EUsarCorrelationIdDoContexto()
    {
        // Arrange
        var filtro = new ConsumeCorrelationIdFilter<MensagemProbe>();
        var correlationId = Guid.NewGuid();
        var (contexto, headers) = SendContextMockExtensions.CriarConsumeContext(new MensagemProbe());
        var next = new Mock<IPipe<ConsumeContext<MensagemProbe>>>();
        contexto.ComCorrelationId(correlationId);
        headers.ComHeader(CorrelationConstants.HeaderName, "   ");
        string? correlationIdCapturado = null;
        next.AoEnviarCapturando(contexto.Object, () => correlationIdCapturado = CorrelationContext.Current);

        // Act
        await filtro.Send(contexto.Object, next.Object);

        // Assert
        next.DeveTerSidoChamado(contexto.Object);
        correlationIdCapturado.ShouldBe(correlationId.ToString());
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

        // Act & Assert
        ProbeContextAssertionExtensions.DeveResponderAoProbeComScopeFilters(filtro.Probe);
    }

    [Fact(DisplayName = "Send deve propagar correlation id e chamar o próximo filtro")]
    [Trait("Infrastructure", "PublishCorrelationIdFilter")]
    public async Task Send_DevePropagarCorrelationIdEChamarNextSend()
    {
        // Arrange
        var filtro = new PublishCorrelationIdFilter<MensagemPublish>();
        var correlationIdEsperado = Guid.NewGuid().ToString();
        var correlationIdEsperadoGuid = Guid.Parse(correlationIdEsperado);
        var (contexto, headers) = SendContextMockExtensions.CriarPublishContextComCorrelationId<MensagemPublish>();
        var next = new Mock<IPipe<PublishContext<MensagemPublish>>>();
        next.AoEnviar(contexto.Object);

        // Act
        using (CorrelationContext.Push(correlationIdEsperado))
            await filtro.Send(contexto.Object, next.Object);

        // Assert
        next.DeveTerSidoChamado(contexto.Object);
        contexto.Object.CorrelationId.ShouldBe(correlationIdEsperadoGuid);
        headers.DeveTerDefinidoHeader(CorrelationConstants.HeaderName, correlationIdEsperado);
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

        // Act & Assert
        ProbeContextAssertionExtensions.DeveResponderAoProbeComScopeFilters(filtro.Probe);
    }

    [Fact(DisplayName = "Send deve propagar correlation id e chamar o próximo filtro")]
    [Trait("Infrastructure", "SendCorrelationIdFilter")]
    public async Task Send_DevePropagarCorrelationIdEChamarNextSend()
    {
        // Arrange
        var filtro = new SendCorrelationIdFilter<MensagemSend>();
        var correlationIdEsperado = Guid.NewGuid().ToString();
        var correlationIdEsperadoGuid = Guid.Parse(correlationIdEsperado);
        var (contexto, headers) = SendContextMockExtensions.CriarSendContextComCorrelationId<MensagemSend>();
        var next = new Mock<IPipe<SendContext<MensagemSend>>>();
        next.AoEnviar(contexto.Object);

        // Act
        using (CorrelationContext.Push(correlationIdEsperado))
            await filtro.Send(contexto.Object, next.Object);

        // Assert
        next.DeveTerSidoChamado(contexto.Object);
        contexto.Object.CorrelationId.ShouldBe(correlationIdEsperadoGuid);
        headers.DeveTerDefinidoHeader(CorrelationConstants.HeaderName, correlationIdEsperado);
    }

    public class MensagemSend { }
}
