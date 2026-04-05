namespace Tests.Domain.ProcessamentoDiagrama.ValueObjects;

public class RecomendacaoBasicaTests
{
    [Fact(DisplayName = "Deve criar recomendacao basica quando valor valido")]
    [Trait("ValueObject", "RecomendacaoBasica")]
    public void Construtor_DeveCriar_QuandoValorValido()
    {
        // Arrange
        var valor = "Implementar circuit breaker";

        // Act
        var recomendacao = new RecomendacaoBasica(valor);

        // Assert
        recomendacao.Valor.ShouldBe(valor);
    }

    [Theory(DisplayName = "Deve lancar excecao quando valor nulo, vazio ou espacos")]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [Trait("ValueObject", "RecomendacaoBasica")]
    public void Construtor_DeveLancarExcecao_QuandoValorInvalido(string? valor)
    {
        // Arrange
        Action acao = () => _ = new RecomendacaoBasica(valor!);

        // Act
        var excecao = Should.Throw<DomainException>(acao);

        // Assert
        excecao.ErrorType.ShouldBe(ErrorType.InvalidInput);
        excecao.Message.ShouldContain("Recomendação básica não pode ser vazia");
    }

    [Fact(DisplayName = "Deve aplicar trim quando valor possui espacos nas extremidades")]
    [Trait("ValueObject", "RecomendacaoBasica")]
    public void Construtor_DeveAplicarTrim_QuandoValorComEspacos()
    {
        // Arrange
        var valor = "  Implementar circuit breaker  ";

        // Act
        var recomendacao = new RecomendacaoBasica(valor);

        // Assert
        recomendacao.Valor.ShouldBe("Implementar circuit breaker");
    }
}
