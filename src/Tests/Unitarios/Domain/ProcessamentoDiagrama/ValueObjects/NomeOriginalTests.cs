namespace Tests.Domain.ProcessamentoDiagrama.ValueObjects;

public class NomeOriginalTests
{
    [Fact(DisplayName = "Deve criar nome original quando valor valido")]
    [Trait("ValueObject", "NomeOriginal")]
    public void Construtor_DeveCriar_QuandoValorValido()
    {
        // Arrange
        var valor = "diagrama-arquitetura.png";

        // Act
        var nomeOriginal = new NomeOriginal(valor);

        // Assert
        nomeOriginal.Valor.ShouldBe(valor);
    }

    [Theory(DisplayName = "Deve lancar excecao quando valor nulo, vazio ou espacos")]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [Trait("ValueObject", "NomeOriginal")]
    public void Construtor_DeveLancarExcecao_QuandoValorInvalido(string? valor)
    {
        // Arrange
        Action acao = () => _ = new NomeOriginal(valor!);

        // Act
        var excecao = Should.Throw<DomainException>(acao);

        // Assert
        excecao.ErrorType.ShouldBe(ErrorType.InvalidInput);
        excecao.Message.ShouldContain("Nome original não pode ser vazio");
    }

    [Fact(DisplayName = "Deve lancar excecao quando valor excede comprimento maximo")]
    [Trait("ValueObject", "NomeOriginal")]
    public void Construtor_DeveLancarExcecao_QuandoExcedeComprimentoMaximo()
    {
        // Arrange
        var valor = new string('a', 501);

        // Act
        Action acao = () => _ = new NomeOriginal(valor);

        // Assert
        acao.DeveLancarExcecaoDeValidacao("não pode exceder 500 caracteres");
    }

    [Fact(DisplayName = "Deve criar nome original com exatamente 500 caracteres")]
    [Trait("ValueObject", "NomeOriginal")]
    public void Construtor_DeveCriar_QuandoValorComExatamente500Caracteres()
    {
        // Arrange
        var valor = new string('a', 500);

        // Act
        var nomeOriginal = new NomeOriginal(valor);

        // Assert
        nomeOriginal.Valor.Length.ShouldBe(500);
    }

    [Fact(DisplayName = "Deve aplicar trim quando valor possui espacos nas extremidades")]
    [Trait("ValueObject", "NomeOriginal")]
    public void Construtor_DeveAplicarTrim_QuandoValorComEspacos()
    {
        // Arrange
        var valor = "  diagrama-arquitetura.png  ";

        // Act
        var nomeOriginal = new NomeOriginal(valor);

        // Assert
        nomeOriginal.Valor.ShouldBe("diagrama-arquitetura.png");
    }

    [Fact(DisplayName = "Deve permitir reconstrução por ORM via construtor privado")]
    [Trait("ValueObject", "NomeOriginal")]
    public void ConstrutorPrivado_DeveInstanciar_ParaReconstrucaoORM()
    {
        // Act
        var instancia = (NomeOriginal)Activator.CreateInstance(typeof(NomeOriginal), nonPublic: true)!;

        // Assert
        instancia.ShouldNotBeNull();
        instancia.Valor.ShouldBe(string.Empty);
    }
}
