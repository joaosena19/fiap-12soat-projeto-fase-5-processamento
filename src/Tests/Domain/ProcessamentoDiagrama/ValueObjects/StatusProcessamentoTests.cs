namespace Tests.Domain.ProcessamentoDiagrama.ValueObjects;

public class StatusProcessamentoTests
{
    [Theory(DisplayName = "Deve criar status de processamento quando valor valido")]
    [InlineData(StatusProcessamentoEnum.AguardandoProcessamento)]
    [InlineData(StatusProcessamentoEnum.EmProcessamento)]
    [InlineData(StatusProcessamentoEnum.Concluido)]
    [InlineData(StatusProcessamentoEnum.Falha)]
    [Trait("ValueObject", "StatusProcessamento")]
    public void Construtor_DeveCriar_QuandoStatusValido(StatusProcessamentoEnum valor)
    {
        // Arrange
        var statusEsperado = valor;

        // Act
        var status = new StatusProcessamento(statusEsperado);

        // Assert
        status.Valor.ShouldBe(statusEsperado);
    }

    [Fact(DisplayName = "Deve lancar excecao quando status invalido")]
    [Trait("ValueObject", "StatusProcessamento")]
    public void Construtor_DeveLancarExcecao_QuandoStatusInvalido()
    {
        // Arrange
        var valorInvalido = (StatusProcessamentoEnum)999;

        // Act
        Action acao = () => _ = new StatusProcessamento(valorInvalido);

        // Assert
        var excecao = Should.Throw<DomainException>(acao);
        excecao.ErrorType.ShouldBe(ErrorType.InvalidInput);
        excecao.Message.ShouldContain("não é válido");
    }
}
