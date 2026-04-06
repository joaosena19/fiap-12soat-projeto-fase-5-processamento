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
        var (contexto, headers) = SendContextMockExtensions.CriarConsumeContext(new MensagemTeste());
        var next = new Mock<IPipe<ConsumeContext<MensagemTeste>>>();
        headers.ComHeader(CorrelationConstants.HeaderName, "header-correlation");
        string? correlationIdCapturado = null;
        next.AoEnviarCapturando(contexto.Object, () => correlationIdCapturado = CorrelationContext.Current);

        // Act
        await filtro.Send(contexto.Object, next.Object);

        // Assert
        next.DeveTerSidoChamado(contexto.Object);
        correlationIdCapturado.ShouldBe("header-correlation");
        CorrelationContext.Current.ShouldBeNull();
    }

    [Fact(DisplayName = "Deve usar correlation id do contexto quando header não existir")]
    [Trait("Infrastructure", "ConsumeCorrelationIdFilter")]
    public async Task Send_DeveUsarCorrelationIdDoContexto_QuandoHeaderAusente()
    {
        // Arrange
        var filtro = new ConsumeCorrelationIdFilter<MensagemTeste>();
        var correlationId = Guid.NewGuid();
        var (contexto, headers) = SendContextMockExtensions.CriarConsumeContext(new MensagemTeste());
        var next = new Mock<IPipe<ConsumeContext<MensagemTeste>>>();
        contexto.ComCorrelationId(correlationId);
        headers.SemHeader(CorrelationConstants.HeaderName);
        string? correlationIdCapturado = null;
        next.AoEnviarCapturando(contexto.Object, () => correlationIdCapturado = CorrelationContext.Current);

        // Act
        await filtro.Send(contexto.Object, next.Object);

        // Assert
        next.DeveTerSidoChamado(contexto.Object);
        correlationIdCapturado.ShouldBe(correlationId.ToString());
    }

    [Fact(DisplayName = "Deve usar correlation id da mensagem quando contexto não fornecer valor")]
    [Trait("Infrastructure", "ConsumeCorrelationIdFilter")]
    public async Task Send_DeveUsarCorrelationIdDaMensagem_QuandoContextoNaoTiverValor()
    {
        // Arrange
        var filtro = new ConsumeCorrelationIdFilter<MensagemTeste>();
        var (contexto, headers) = SendContextMockExtensions.CriarConsumeContext(new MensagemTeste { CorrelationId = "mensagem-correlation" });
        var next = new Mock<IPipe<ConsumeContext<MensagemTeste>>>();
        contexto.ComCorrelationId(null);
        headers.SemHeader(CorrelationConstants.HeaderName);
        string? correlationIdCapturado = null;
        next.AoEnviarCapturando(contexto.Object, () => correlationIdCapturado = CorrelationContext.Current);

        // Act
        await filtro.Send(contexto.Object, next.Object);

        // Assert
        next.DeveTerSidoChamado(contexto.Object);
        correlationIdCapturado.ShouldBe("mensagem-correlation");
    }

    [Fact(DisplayName = "Deve gerar novo correlation id quando nenhum valor estiver disponível")]
    [Trait("Infrastructure", "ConsumeCorrelationIdFilter")]
    public async Task Send_DeveGerarCorrelationId_QuandoNenhumaOrigemDisponivel()
    {
        // Arrange
        var filtro = new ConsumeCorrelationIdFilter<MensagemTeste>();
        var (contexto, headers) = SendContextMockExtensions.CriarConsumeContext(new MensagemTeste());
        var next = new Mock<IPipe<ConsumeContext<MensagemTeste>>>();
        contexto.ComCorrelationId(null);
        headers.SemHeader(CorrelationConstants.HeaderName);
        string? correlationIdCapturado = null;
        next.AoEnviarCapturando(contexto.Object, () => correlationIdCapturado = CorrelationContext.Current);

        // Act
        await filtro.Send(contexto.Object, next.Object);

        // Assert
        next.DeveTerSidoChamado(contexto.Object);
        Guid.TryParse(correlationIdCapturado, out _).ShouldBeTrue();
    }

    public class MensagemTeste
    {
        public string? CorrelationId { get; init; }
    }
}