using Infrastructure.Messaging.Filters;
using Infrastructure.Monitoramento.Correlation;
using MassTransit;

namespace Tests.Infrastructure.Messaging.Filters;

public class CorrelationIdFiltersTests
{
    [Fact(DisplayName = "Deve propagar correlation id atual no helper quando valor é GUID")]
    [Trait("Infrastructure", "CorrelationIdFilters")]
    public void AplicarCorrelationId_DevePropagarHeaderECorrelationId_QuandoValorAtualEhGuid()
    {
        // Arrange
        var contexto = new Mock<SendContext<MensagemTeste>>();
        var headers = new Mock<SendHeaders>();
        var correlationId = Guid.NewGuid();

        contexto.Setup(x => x.Headers).Returns(headers.Object);
        contexto.SetupProperty(x => x.CorrelationId);

        // Act
        using (CorrelationContext.Push(correlationId.ToString()))
            CorrelationIdFilterHelper.AplicarCorrelationId(contexto.Object);

        // Assert
        contexto.Object.CorrelationId.ShouldBe(correlationId);
        headers.Verify(x => x.Set(CorrelationConstants.HeaderName, correlationId.ToString()), Times.Once);
    }

    [Fact(DisplayName = "Deve propagar header e não definir CorrelationId quando valor não é GUID")]
    [Trait("Infrastructure", "CorrelationIdFilters")]
    public void AplicarCorrelationId_DevePropagarApenasHeader_QuandoValorAtualNaoEhGuid()
    {
        // Arrange
        var contexto = new Mock<SendContext<MensagemTeste>>();
        var headers = new Mock<SendHeaders>();

        contexto.Setup(x => x.Headers).Returns(headers.Object);
        contexto.SetupProperty(x => x.CorrelationId);

        // Act
        using (CorrelationContext.Push("correlation-nao-guid"))
            CorrelationIdFilterHelper.AplicarCorrelationId(contexto.Object);

        // Assert
        contexto.Object.CorrelationId.ShouldBeNull();
        headers.Verify(x => x.Set(CorrelationConstants.HeaderName, "correlation-nao-guid"), Times.Once);
    }

    [Fact(DisplayName = "Deve gerar correlation id quando contexto atual não possui valor")]
    [Trait("Infrastructure", "CorrelationIdFilters")]
    public void AplicarCorrelationId_DeveGerarValor_QuandoCorrelationContextNaoDefinido()
    {
        // Arrange
        var contexto = new Mock<SendContext<MensagemTeste>>();
        var headers = new Mock<SendHeaders>();

        contexto.Setup(x => x.Headers).Returns(headers.Object);
        contexto.SetupProperty(x => x.CorrelationId);
        string? correlationIdHeader = null;

        headers.Setup(x => x.Set(CorrelationConstants.HeaderName, It.IsAny<string>()))
            .Callback<string, object>((_, valor) => correlationIdHeader = valor.ToString());

        // Act
        CorrelationIdFilterHelper.AplicarCorrelationId(contexto.Object);

        // Assert
        correlationIdHeader.ShouldNotBeNullOrWhiteSpace();
        Guid.TryParse(correlationIdHeader, out _).ShouldBeTrue();
        contexto.Object.CorrelationId.ShouldNotBeNull();
    }

    public class MensagemTeste
    {
        public string? CorrelationId { get; init; }
    }
}
