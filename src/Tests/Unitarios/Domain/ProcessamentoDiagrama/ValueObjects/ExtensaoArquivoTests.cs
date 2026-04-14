namespace Tests.Domain.ProcessamentoDiagrama.ValueObjects;

public class ExtensaoArquivoTests
{
    [Fact(DisplayName = "Deve criar extensao quando valor valido")]
    [Trait("ValueObject", "ExtensaoArquivo")]
    public void Construtor_DeveCriar_QuandoValorValido()
    {
        // Arrange
        var valor = "png";

        // Act
        var extensao = new ExtensaoArquivo(valor);

        // Assert
        extensao.Valor.ShouldBe(valor);
    }

    [Theory(DisplayName = "Deve lancar excecao quando valor nulo, vazio ou espacos")]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [Trait("ValueObject", "ExtensaoArquivo")]
    public void Construtor_DeveLancarExcecao_QuandoValorInvalido(string? valor)
    {
        // Arrange
        Action acao = () => _ = new ExtensaoArquivo(valor!);

        // Act
        var excecao = Should.Throw<DomainException>(acao);

        // Assert
        excecao.ErrorType.ShouldBe(ErrorType.InvalidInput);
        excecao.Message.ShouldContain("Extensão do arquivo não pode ser vazia");
    }

    [Fact(DisplayName = "Deve lancar excecao quando valor excede comprimento maximo")]
    [Trait("ValueObject", "ExtensaoArquivo")]
    public void Construtor_DeveLancarExcecao_QuandoExcedeComprimentoMaximo()
    {
        // Arrange
        var valor = new string('a', 21);

        // Act
        Action acao = () => _ = new ExtensaoArquivo(valor);

        // Assert
        acao.DeveLancarExcecaoDeValidacao("não pode exceder 20 caracteres");
    }

    [Fact(DisplayName = "Deve criar extensao com exatamente 20 caracteres")]
    [Trait("ValueObject", "ExtensaoArquivo")]
    public void Construtor_DeveCriar_QuandoValorComExatamente20Caracteres()
    {
        // Arrange
        var valor = new string('a', 20);

        // Act
        var extensao = new ExtensaoArquivo(valor);

        // Assert
        extensao.Valor.Length.ShouldBe(20);
    }

    [Fact(DisplayName = "Deve aplicar trim quando valor possui espacos nas extremidades")]
    [Trait("ValueObject", "ExtensaoArquivo")]
    public void Construtor_DeveAplicarTrim_QuandoValorComEspacos()
    {
        // Arrange
        var valor = "  png  ";

        // Act
        var extensao = new ExtensaoArquivo(valor);

        // Assert
        extensao.Valor.ShouldBe("png");
    }

    [Fact(DisplayName = "Deve permitir reconstrução por ORM via construtor privado")]
    [Trait("ValueObject", "ExtensaoArquivo")]
    public void ConstrutorPrivado_DeveInstanciar_ParaReconstrucaoORM()
    {
        // Act
        var instancia = (ExtensaoArquivo)Activator.CreateInstance(typeof(ExtensaoArquivo), nonPublic: true)!;

        // Assert
        instancia.ShouldNotBeNull();
        instancia.Valor.ShouldBe(string.Empty);
    }
}
