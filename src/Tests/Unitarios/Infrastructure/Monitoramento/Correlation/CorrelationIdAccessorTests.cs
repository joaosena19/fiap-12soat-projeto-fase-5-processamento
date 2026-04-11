using Infrastructure.Monitoramento;
using Infrastructure.Monitoramento.Correlation;

namespace Tests.Infrastructure.Monitoramento.Correlation;

public class CorrelationIdAccessorTests
{
    [Fact(DisplayName = "Deve retornar correlation id atual quando contexto possui valor")]
    [Trait("Infrastructure", "CorrelationIdAccessor")]
    public void GetCorrelationId_DeveRetornarValorAtual_QuandoContextoPossuiCorrelationId()
    {
        // Arrange
        var accessor = new CorrelationIdAccessor();
        var correlationId = Guid.NewGuid().ToString();

        // Act
        using (CorrelationContext.Push(correlationId))
        {
            var resultado = accessor.GetCorrelationId();

            // Assert
            resultado.ShouldBe(correlationId);
        }
    }

    [Fact(DisplayName = "Deve gerar novo correlation id quando contexto não possui valor")]
    [Trait("Infrastructure", "CorrelationIdAccessor")]
    public void GetCorrelationId_DeveGerarNovoGuid_QuandoContextoNaoPossuiCorrelationId()
    {
        // Arrange
        var accessor = new CorrelationIdAccessor();

        // Act
        var resultado = accessor.GetCorrelationId();

        // Assert
        resultado.ShouldNotBeNullOrWhiteSpace();
        Guid.TryParse(resultado, out _).ShouldBeTrue();
    }
}
