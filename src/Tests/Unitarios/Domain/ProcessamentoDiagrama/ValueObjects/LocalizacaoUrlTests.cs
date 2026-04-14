namespace Tests.Domain.ProcessamentoDiagrama.ValueObjects;

public class LocalizacaoUrlTests
{
    [Fact(DisplayName = "Deve criar localizacao url quando valor valido")]
    [Trait("ValueObject", "LocalizacaoUrl")]
    public void Construtor_DeveCriar_QuandoValorValido()
    {
        // Arrange
        var valor = "s3://bucket/diagramas/arquivo-123.png";

        // Act
        var localizacaoUrl = new LocalizacaoUrl(valor);

        // Assert
        localizacaoUrl.Valor.ShouldBe(valor);
    }

    [Theory(DisplayName = "Deve lancar excecao quando valor nulo, vazio ou espacos")]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [Trait("ValueObject", "LocalizacaoUrl")]
    public void Construtor_DeveLancarExcecao_QuandoValorInvalido(string? valor)
    {
        // Arrange
        Action acao = () => _ = new LocalizacaoUrl(valor!);

        // Act
        var excecao = Should.Throw<DomainException>(acao);

        // Assert
        excecao.ErrorType.ShouldBe(ErrorType.InvalidInput);
        excecao.Message.ShouldContain("Localização URL não pode ser vazia");
    }

    [Fact(DisplayName = "Deve lancar excecao quando valor excede comprimento maximo")]
    [Trait("ValueObject", "LocalizacaoUrl")]
    public void Construtor_DeveLancarExcecao_QuandoExcedeComprimentoMaximo()
    {
        // Arrange
        var valor = new string('a', 501);

        // Act
        Action acao = () => _ = new LocalizacaoUrl(valor);

        // Assert
        acao.DeveLancarExcecaoDeValidacao("não pode exceder 500 caracteres");
    }

    [Fact(DisplayName = "Deve criar localizacao url com exatamente 500 caracteres")]
    [Trait("ValueObject", "LocalizacaoUrl")]
    public void Construtor_DeveCriar_QuandoValorComExatamente500Caracteres()
    {
        // Arrange
        var valor = new string('a', 500);

        // Act
        var localizacaoUrl = new LocalizacaoUrl(valor);

        // Assert
        localizacaoUrl.Valor.Length.ShouldBe(500);
    }

    [Fact(DisplayName = "Deve aplicar trim quando valor possui espacos nas extremidades")]
    [Trait("ValueObject", "LocalizacaoUrl")]
    public void Construtor_DeveAplicarTrim_QuandoValorComEspacos()
    {
        // Arrange
        var valor = "  s3://bucket/arquivo.png  ";

        // Act
        var localizacaoUrl = new LocalizacaoUrl(valor);

        // Assert
        localizacaoUrl.Valor.ShouldBe("s3://bucket/arquivo.png");
    }

    [Fact(DisplayName = "Deve permitir reconstrução por ORM via construtor privado")]
    [Trait("ValueObject", "LocalizacaoUrl")]
    public void ConstrutorPrivado_DeveInstanciar_ParaReconstrucaoORM()
    {
        // Act
        var instancia = (LocalizacaoUrl)Activator.CreateInstance(typeof(LocalizacaoUrl), nonPublic: true)!;

        // Assert
        instancia.ShouldNotBeNull();
        instancia.Valor.ShouldBe(string.Empty);
    }
}
