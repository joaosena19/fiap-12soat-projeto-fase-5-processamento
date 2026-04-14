namespace Tests.Domain.ProcessamentoDiagrama.ValueObjects;

public class NomeFisicoTests
{
    [Fact(DisplayName = "Deve criar nome fisico quando valor valido")]
    [Trait("ValueObject", "NomeFisico")]
    public void Construtor_DeveCriar_QuandoValorValido()
    {
        // Arrange
        var valor = "diagrama-123.png";

        // Act
        var nomeFisico = new NomeFisico(valor);

        // Assert
        nomeFisico.Valor.ShouldBe(valor);
    }

    [Theory(DisplayName = "Deve lancar excecao quando valor nulo, vazio ou espacos")]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [Trait("ValueObject", "NomeFisico")]
    public void Construtor_DeveLancarExcecao_QuandoValorInvalido(string? valor)
    {
        // Arrange
        Action acao = () => _ = new NomeFisico(valor!);

        // Act
        var excecao = Should.Throw<DomainException>(acao);

        // Assert
        excecao.ErrorType.ShouldBe(ErrorType.InvalidInput);
        excecao.Message.ShouldContain("Nome físico não pode ser vazio");
    }

    [Fact(DisplayName = "Deve lancar excecao quando valor excede comprimento maximo")]
    [Trait("ValueObject", "NomeFisico")]
    public void Construtor_DeveLancarExcecao_QuandoExcedeComprimentoMaximo()
    {
        // Arrange
        var valor = new string('a', 201);

        // Act
        Action acao = () => _ = new NomeFisico(valor);

        // Assert
        acao.DeveLancarExcecaoDeValidacao("não pode exceder 200 caracteres");
    }

    [Fact(DisplayName = "Deve criar nome fisico com exatamente 200 caracteres")]
    [Trait("ValueObject", "NomeFisico")]
    public void Construtor_DeveCriar_QuandoValorComExatamente200Caracteres()
    {
        // Arrange
        var valor = new string('a', 200);

        // Act
        var nomeFisico = new NomeFisico(valor);

        // Assert
        nomeFisico.Valor.Length.ShouldBe(200);
    }

    [Fact(DisplayName = "Deve aplicar trim quando valor possui espacos nas extremidades")]
    [Trait("ValueObject", "NomeFisico")]
    public void Construtor_DeveAplicarTrim_QuandoValorComEspacos()
    {
        // Arrange
        var valor = "  diagrama-123.png  ";

        // Act
        var nomeFisico = new NomeFisico(valor);

        // Assert
        nomeFisico.Valor.ShouldBe("diagrama-123.png");
    }

    [Fact(DisplayName = "Deve permitir reconstrução por ORM via construtor privado")]
    [Trait("ValueObject", "NomeFisico")]
    public void ConstrutorPrivado_DeveInstanciar_ParaReconstrucaoORM()
    {
        // Act
        var instancia = (NomeFisico)Activator.CreateInstance(typeof(NomeFisico), nonPublic: true)!;

        // Assert
        instancia.ShouldNotBeNull();
        instancia.Valor.ShouldBe(string.Empty);
    }
}
