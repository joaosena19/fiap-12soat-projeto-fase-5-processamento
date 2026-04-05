using Infrastructure.Monitoramento.Correlation;

namespace Tests.Infrastructure.Monitoramento.Correlation;

public class CorrelationContextTests
{
    [Fact(DisplayName = "Deve definir correlation id durante o escopo")]
    [Trait("Infrastructure", "CorrelationContext")]
    public void Push_DeveDefinirCorrelationId_DuranteEscopo()
    {
        // Arrange
        var correlationId = Guid.NewGuid().ToString();

        // Act
        using (CorrelationContext.Push(correlationId))
        {
            // Assert
            CorrelationContext.Current.ShouldBe(correlationId);
        }
    }

    [Fact(DisplayName = "Deve restaurar valor anterior ao encerrar escopo")]
    [Trait("Infrastructure", "CorrelationContext")]
    public void Push_DeveRestaurarValorAnterior_QuandoEscopoEncerrar()
    {
        // Arrange
        var valorInicial = "correlation-anterior";

        // Act
        using (CorrelationContext.Push(valorInicial))
        using (CorrelationContext.Push("correlation-nova"))
            CorrelationContext.Current.ShouldBe("correlation-nova");

        // Assert
        CorrelationContext.Current.ShouldBeNull();
    }

    [Fact(DisplayName = "Não deve lançar erro ao descartar escopo duas vezes")]
    [Trait("Infrastructure", "CorrelationContext")]
    public void Dispose_NaoDeveLancarErro_QuandoChamadoMaisDeUmaVez()
    {
        // Arrange
        var escopo = CorrelationContext.Push("correlation-id");

        // Act
        escopo.Dispose();
        var acao = () => escopo.Dispose();

        // Assert
        acao.ShouldNotThrow();
    }
}
