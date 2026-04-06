namespace Tests.Domain.ProcessamentoDiagrama.ValueObjects;

public class ComponenteIdentificadoTests
{
    [Fact(DisplayName = "Deve criar componente identificado quando valor valido")]
    [Trait("ValueObject", "ComponenteIdentificado")]
    public void Construtor_DeveCriar_QuandoValorValido()
    {
        // Arrange
        var valor = "API Gateway";

        // Act
        var componente = new ComponenteIdentificado(valor);

        // Assert
        componente.Valor.ShouldBe(valor);
    }

    [Theory(DisplayName = "Deve lancar excecao quando valor nulo, vazio ou espacos")]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [Trait("ValueObject", "ComponenteIdentificado")]
    public void Construtor_DeveLancarExcecao_QuandoValorInvalido(string? valor)
    {
        // Arrange
        Action acao = () => _ = new ComponenteIdentificado(valor!);

        // Act
        var excecao = Should.Throw<DomainException>(acao);

        // Assert
        excecao.ErrorType.ShouldBe(ErrorType.InvalidInput);
        excecao.Message.ShouldContain("Componente identificado não pode ser vazio");
    }

    [Fact(DisplayName = "Deve aplicar trim quando valor possui espacos nas extremidades")]
    [Trait("ValueObject", "ComponenteIdentificado")]
    public void Construtor_DeveAplicarTrim_QuandoValorComEspacos()
    {
        // Arrange
        var valor = "  API Gateway  ";

        // Act
        var componente = new ComponenteIdentificado(valor);

        // Assert
        componente.Valor.ShouldBe("API Gateway");
    }

    [Fact(DisplayName = "Deve permitir reconstrução por ORM via construtor privado")]
    [Trait("ValueObject", "ComponenteIdentificado")]
    public void ConstrutorPrivado_DeveInstanciar_ParaReconstrucaoORM()
    {
        // Act
        var instancia = (ComponenteIdentificado)Activator.CreateInstance(typeof(ComponenteIdentificado), nonPublic: true)!;

        // Assert
        instancia.ShouldNotBeNull();
        instancia.Valor.ShouldBe(string.Empty);
    }
}
