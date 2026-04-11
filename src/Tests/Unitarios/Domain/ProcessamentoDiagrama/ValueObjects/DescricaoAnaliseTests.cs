namespace Tests.Domain.ProcessamentoDiagrama.ValueObjects;

public class DescricaoAnaliseTests
{
    [Fact(DisplayName = "Deve criar descricao de analise quando valor valido")]
    [Trait("ValueObject", "DescricaoAnalise")]
    public void Construtor_DeveCriar_QuandoValorValido()
    {
        // Arrange
        var valor = "Analise detalhada do diagrama";

        // Act
        var descricao = new DescricaoAnalise(valor);

        // Assert
        descricao.Valor.ShouldBe(valor);
    }

    [Theory(DisplayName = "Deve lancar excecao quando valor nulo, vazio ou espacos")]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [Trait("ValueObject", "DescricaoAnalise")]
    public void Construtor_DeveLancarExcecao_QuandoValorInvalido(string? valor)
    {
        // Arrange
        Action acao = () => _ = new DescricaoAnalise(valor!);

        // Act
        var excecao = Should.Throw<DomainException>(acao);

        // Assert
        excecao.ErrorType.ShouldBe(ErrorType.InvalidInput);
        excecao.Message.ShouldContain("Descrição da análise não pode ser vazia");
    }

    [Fact(DisplayName = "Deve lancar excecao quando valor excede comprimento maximo")]
    [Trait("ValueObject", "DescricaoAnalise")]
    public void Construtor_DeveLancarExcecao_QuandoExcedeComprimentoMaximo()
    {
        // Arrange
        var valor = new string('a', 10001);

        // Act
        Action acao = () => _ = new DescricaoAnalise(valor);

        // Assert
        acao.DeveLancarExcecaoDeValidacao("não pode exceder 10000 caracteres");
    }

    [Fact(DisplayName = "Deve criar descricao com exatamente 10000 caracteres")]
    [Trait("ValueObject", "DescricaoAnalise")]
    public void Construtor_DeveCriar_QuandoValorComExatamente10000Caracteres()
    {
        // Arrange
        var valor = new string('a', 10000);

        // Act
        var descricao = new DescricaoAnalise(valor);

        // Assert
        descricao.Valor.Length.ShouldBe(10000);
    }

    [Fact(DisplayName = "Deve aplicar trim quando valor possui espacos nas extremidades")]
    [Trait("ValueObject", "DescricaoAnalise")]
    public void Construtor_DeveAplicarTrim_QuandoValorComEspacos()
    {
        // Arrange
        var valor = "  Analise detalhada  ";

        // Act
        var descricao = new DescricaoAnalise(valor);

        // Assert
        descricao.Valor.ShouldBe("Analise detalhada");
    }
}
