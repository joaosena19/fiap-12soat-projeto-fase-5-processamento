namespace Tests.Domain.ProcessamentoDiagrama.ValueObjects;

public class TentativasProcessamentoTests
{
    [Fact(DisplayName = "Deve criar tentativas de processamento com valor zero")]
    [Trait("ValueObject", "TentativasProcessamento")]
    public void Construtor_DeveCriar_QuandoValorZero()
    {
        // Arrange
        var valor = 0;

        // Act
        var tentativas = new TentativasProcessamento(valor);

        // Assert
        tentativas.Valor.ShouldBe(valor);
    }

    [Fact(DisplayName = "Deve criar tentativas de processamento com valor positivo")]
    [Trait("ValueObject", "TentativasProcessamento")]
    public void Construtor_DeveCriar_QuandoValorPositivo()
    {
        // Arrange
        var valor = 3;

        // Act
        var tentativas = new TentativasProcessamento(valor);

        // Assert
        tentativas.Valor.ShouldBe(valor);
    }

    [Fact(DisplayName = "Deve lancar excecao quando valor negativo")]
    [Trait("ValueObject", "TentativasProcessamento")]
    public void Construtor_DeveLancarExcecao_QuandoValorNegativo()
    {
        // Arrange
        var valor = -1;

        // Act
        Action acao = () => _ = new TentativasProcessamento(valor);

        // Assert
        var excecao = Should.Throw<DomainException>(acao);
        excecao.ErrorType.ShouldBe(ErrorType.InvalidInput);
        excecao.Message.ShouldContain("não podem ser negativas");
    }

    [Fact(DisplayName = "Deve incrementar tentativas quando metodo incrementar for chamado")]
    [Trait("ValueObject", "TentativasProcessamento")]
    public void Incrementar_DeveIncrementar_QuandoChamado()
    {
        // Arrange
        var tentativas = new TentativasProcessamento(2);

        // Act
        var tentativasIncrementadas = tentativas.Incrementar();

        // Assert
        tentativasIncrementadas.Valor.ShouldBe(3);
    }

    [Fact(DisplayName = "Deve retornar nova instancia ao incrementar")]
    [Trait("ValueObject", "TentativasProcessamento")]
    public void Incrementar_DeveRetornarNovaInstancia_QuandoChamado()
    {
        // Arrange
        var tentativas = new TentativasProcessamento(1);

        // Act
        var tentativasIncrementadas = tentativas.Incrementar();

        // Assert
        ReferenceEquals(tentativas, tentativasIncrementadas).ShouldBeFalse();
    }
}
