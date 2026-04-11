namespace Tests.Domain.ProcessamentoDiagrama.ValueObjects;

public class RiscoArquiteturalTests
{
    [Fact(DisplayName = "Deve criar risco arquitetural quando valor valido")]
    [Trait("ValueObject", "RiscoArquitetural")]
    public void Construtor_DeveCriar_QuandoValorValido()
    {
        // Arrange
        var valor = "Ponto unico de falha";

        // Act
        var risco = new RiscoArquitetural(valor);

        // Assert
        risco.Valor.ShouldBe(valor);
    }

    [Theory(DisplayName = "Deve lancar excecao quando valor nulo, vazio ou espacos")]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [Trait("ValueObject", "RiscoArquitetural")]
    public void Construtor_DeveLancarExcecao_QuandoValorInvalido(string? valor)
    {
        // Arrange
        Action acao = () => _ = new RiscoArquitetural(valor!);

        // Act
        var excecao = Should.Throw<DomainException>(acao);

        // Assert
        excecao.ErrorType.ShouldBe(ErrorType.InvalidInput);
        excecao.Message.ShouldContain("Risco arquitetural não pode ser vazio");
    }

    [Fact(DisplayName = "Deve aplicar trim quando valor possui espacos nas extremidades")]
    [Trait("ValueObject", "RiscoArquitetural")]
    public void Construtor_DeveAplicarTrim_QuandoValorComEspacos()
    {
        // Arrange
        var valor = "  Ponto unico de falha  ";

        // Act
        var risco = new RiscoArquitetural(valor);

        // Assert
        risco.Valor.ShouldBe("Ponto unico de falha");
    }

    [Fact(DisplayName = "Deve permitir reconstrução por ORM via construtor privado")]
    [Trait("ValueObject", "RiscoArquitetural")]
    public void ConstrutorPrivado_DeveInstanciar_ParaReconstrucaoORM()
    {
        // Act
        var instancia = (RiscoArquitetural)Activator.CreateInstance(typeof(RiscoArquitetural), nonPublic: true)!;

        // Assert
        instancia.ShouldNotBeNull();
        instancia.Valor.ShouldBe(string.Empty);
    }
}
