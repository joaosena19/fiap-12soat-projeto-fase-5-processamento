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
        var correlationId = Guid.NewGuid();
        var (contexto, headers) = SendContextMockExtensions.CriarSendContextComCorrelationId<MensagemTeste>();

        // Act
        using (CorrelationContext.Push(correlationId.ToString()))
            CorrelationIdFilterHelper.AplicarCorrelationId(contexto.Object);

        // Assert
        contexto.Object.CorrelationId.ShouldBe(correlationId);
        headers.DeveTerDefinidoHeader(CorrelationConstants.HeaderName, correlationId.ToString());
    }

    [Fact(DisplayName = "Deve propagar header e não definir CorrelationId quando valor não é GUID")]
    [Trait("Infrastructure", "CorrelationIdFilters")]
    public void AplicarCorrelationId_DevePropagarApenasHeader_QuandoValorAtualNaoEhGuid()
    {
        // Arrange
        var (contexto, headers) = SendContextMockExtensions.CriarSendContextComCorrelationId<MensagemTeste>();

        // Act
        using (CorrelationContext.Push("correlation-nao-guid"))
            CorrelationIdFilterHelper.AplicarCorrelationId(contexto.Object);

        // Assert
        contexto.Object.CorrelationId.ShouldBeNull();
        headers.DeveTerDefinidoHeader(CorrelationConstants.HeaderName, "correlation-nao-guid");
    }

    [Fact(DisplayName = "Deve gerar correlation id quando contexto atual não possui valor")]
    [Trait("Infrastructure", "CorrelationIdFilters")]
    public void AplicarCorrelationId_DeveGerarValor_QuandoCorrelationContextNaoDefinido()
    {
        // Arrange
        var (contexto, headers) = SendContextMockExtensions.CriarSendContextComCorrelationId<MensagemTeste>();
        var obterCorrelationIdHeader = headers.CapturarSet(CorrelationConstants.HeaderName);

        // Act
        CorrelationIdFilterHelper.AplicarCorrelationId(contexto.Object);

        // Assert
        var correlationIdHeader = obterCorrelationIdHeader();
        correlationIdHeader.ShouldNotBeNullOrWhiteSpace();
        Guid.TryParse(correlationIdHeader, out _).ShouldBeTrue();
        contexto.Object.CorrelationId.ShouldNotBeNull();
    }

    public class MensagemTeste
    {
        public string? CorrelationId { get; init; }
    }
}
