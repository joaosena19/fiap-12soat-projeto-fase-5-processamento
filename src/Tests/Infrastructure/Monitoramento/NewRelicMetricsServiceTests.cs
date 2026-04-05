using Infrastructure.Monitoramento;
using Infrastructure.Monitoramento.Correlation;

namespace Tests.Infrastructure.Monitoramento;

public class NewRelicMetricsServiceTests
{
    [Fact(DisplayName = "Não deve lançar erro ao registrar processamento iniciado")]
    [Trait("Infrastructure", "NewRelicMetricsService")]
    public void RegistrarProcessamentoIniciado_NaoDeveLancarErro_QuandoChamado()
    {
        // Arrange
        var service = new NewRelicMetricsService();

        // Act
        var acao = () => service.RegistrarProcessamentoIniciado(Guid.NewGuid());

        // Assert
        acao.ShouldNotThrow();
    }

    [Fact(DisplayName = "Não deve lançar erro ao registrar processamento concluído")]
    [Trait("Infrastructure", "NewRelicMetricsService")]
    public void RegistrarProcessamentoConcluido_NaoDeveLancarErro_QuandoChamado()
    {
        // Arrange
        var service = new NewRelicMetricsService();

        // Act
        var acao = () => service.RegistrarProcessamentoConcluido(Guid.NewGuid(), 123);

        // Assert
        acao.ShouldNotThrow();
    }

    [Fact(DisplayName = "Não deve lançar erro ao registrar processamento com falha")]
    [Trait("Infrastructure", "NewRelicMetricsService")]
    public void RegistrarProcessamentoFalha_NaoDeveLancarErro_QuandoChamado()
    {
        // Arrange
        var service = new NewRelicMetricsService();

        // Act
        var acao = () => service.RegistrarProcessamentoFalha(Guid.NewGuid(), "erro de teste", 2);

        // Assert
        acao.ShouldNotThrow();
    }

    [Fact(DisplayName = "Não deve lançar erro quando correlation id está presente no contexto")]
    [Trait("Infrastructure", "NewRelicMetricsService")]
    public void RegistrarProcessamentoConcluido_NaoDeveLancarErro_QuandoCorrelationIdPresente()
    {
        // Arrange
        var service = new NewRelicMetricsService();

        // Act
        using (CorrelationContext.Push(Guid.NewGuid().ToString()))
        {
            var acao = () => service.RegistrarProcessamentoConcluido(Guid.NewGuid(), 456);

            // Assert
            acao.ShouldNotThrow();
        }
    }
}
