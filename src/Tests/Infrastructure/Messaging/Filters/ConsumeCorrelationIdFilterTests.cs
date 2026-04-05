using Infrastructure.Messaging.Filters;
using Infrastructure.Monitoramento.Correlation;
using MassTransit;

namespace Tests.Infrastructure.Messaging.Filters;

public class ConsumeCorrelationIdFilterTests
{
    [Fact(DisplayName = "Deve usar correlation id do header quando presente")]
    [Trait("Infrastructure", "ConsumeCorrelationIdFilter")]
    public async Task Send_DeveUsarHeader_QuandoHeaderPresente()
    {
        // Arrange
        var filtro = new ConsumeCorrelationIdFilter<MensagemTeste>();
        var headers = new Mock<Headers>();
        var contexto = new Mock<ConsumeContext<MensagemTeste>>();
        var next = new Mock<IPipe<ConsumeContext<MensagemTeste>>>();
        object? valorHeader = "header-correlation";

        contexto.SetupGet(item => item.Headers).Returns(headers.Object);
        contexto.SetupGet(item => item.Message).Returns(new MensagemTeste());
        headers.Setup(item => item.TryGetHeader(CorrelationConstants.HeaderName, out valorHeader)).Returns(true);
        next.Setup(item => item.Send(contexto.Object)).Returns(() =>
        {
            CorrelationContext.Current.ShouldBe("header-correlation");
            return Task.CompletedTask;
        });

        // Act
        await filtro.Send(contexto.Object, next.Object);

        // Assert
        next.Verify(item => item.Send(contexto.Object), Times.Once);
        CorrelationContext.Current.ShouldBeNull();
    }

    [Fact(DisplayName = "Deve usar correlation id do contexto quando header não existir")]
    [Trait("Infrastructure", "ConsumeCorrelationIdFilter")]
    public async Task Send_DeveUsarCorrelationIdDoContexto_QuandoHeaderAusente()
    {
        // Arrange
        var filtro = new ConsumeCorrelationIdFilter<MensagemTeste>();
        var headers = new Mock<Headers>();
        var contexto = new Mock<ConsumeContext<MensagemTeste>>();
        var next = new Mock<IPipe<ConsumeContext<MensagemTeste>>>();
        object? valorHeader = null;
        var correlationId = Guid.NewGuid();

        contexto.SetupGet(item => item.Headers).Returns(headers.Object);
        contexto.SetupGet(item => item.Message).Returns(new MensagemTeste());
        contexto.SetupGet(item => item.CorrelationId).Returns(correlationId);
        headers.Setup(item => item.TryGetHeader(CorrelationConstants.HeaderName, out valorHeader)).Returns(false);
        next.Setup(item => item.Send(contexto.Object)).Returns(() =>
        {
            CorrelationContext.Current.ShouldBe(correlationId.ToString());
            return Task.CompletedTask;
        });

        // Act
        await filtro.Send(contexto.Object, next.Object);

        // Assert
        next.Verify(item => item.Send(contexto.Object), Times.Once);
    }

    [Fact(DisplayName = "Deve usar correlation id da mensagem quando contexto não fornecer valor")]
    [Trait("Infrastructure", "ConsumeCorrelationIdFilter")]
    public async Task Send_DeveUsarCorrelationIdDaMensagem_QuandoContextoNaoTiverValor()
    {
        // Arrange
        var filtro = new ConsumeCorrelationIdFilter<MensagemTeste>();
        var headers = new Mock<Headers>();
        var contexto = new Mock<ConsumeContext<MensagemTeste>>();
        var next = new Mock<IPipe<ConsumeContext<MensagemTeste>>>();
        object? valorHeader = null;

        contexto.SetupGet(item => item.Headers).Returns(headers.Object);
        contexto.SetupGet(item => item.Message).Returns(new MensagemTeste { CorrelationId = "mensagem-correlation" });
        contexto.SetupGet(item => item.CorrelationId).Returns((Guid?)null);
        headers.Setup(item => item.TryGetHeader(CorrelationConstants.HeaderName, out valorHeader)).Returns(false);
        next.Setup(item => item.Send(contexto.Object)).Returns(() =>
        {
            CorrelationContext.Current.ShouldBe("mensagem-correlation");
            return Task.CompletedTask;
        });

        // Act
        await filtro.Send(contexto.Object, next.Object);

        // Assert
        next.Verify(item => item.Send(contexto.Object), Times.Once);
    }

    [Fact(DisplayName = "Deve gerar novo correlation id quando nenhum valor estiver disponível")]
    [Trait("Infrastructure", "ConsumeCorrelationIdFilter")]
    public async Task Send_DeveGerarCorrelationId_QuandoNenhumaOrigemDisponivel()
    {
        // Arrange
        var filtro = new ConsumeCorrelationIdFilter<MensagemTeste>();
        var headers = new Mock<Headers>();
        var contexto = new Mock<ConsumeContext<MensagemTeste>>();
        var next = new Mock<IPipe<ConsumeContext<MensagemTeste>>>();
        object? valorHeader = null;

        contexto.SetupGet(item => item.Headers).Returns(headers.Object);
        contexto.SetupGet(item => item.Message).Returns(new MensagemTeste());
        contexto.SetupGet(item => item.CorrelationId).Returns((Guid?)null);
        headers.Setup(item => item.TryGetHeader(CorrelationConstants.HeaderName, out valorHeader)).Returns(false);
        next.Setup(item => item.Send(contexto.Object)).Returns(() =>
        {
            Guid.TryParse(CorrelationContext.Current, out _).ShouldBeTrue();
            return Task.CompletedTask;
        });

        // Act
        await filtro.Send(contexto.Object, next.Object);

        // Assert
        next.Verify(item => item.Send(contexto.Object), Times.Once);
    }

    public class MensagemTeste
    {
        public string? CorrelationId { get; init; }
    }
}