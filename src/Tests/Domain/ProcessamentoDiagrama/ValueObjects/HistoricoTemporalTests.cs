namespace Tests.Domain.ProcessamentoDiagrama.ValueObjects;

public class HistoricoTemporalTests
{
    [Fact(DisplayName = "Deve criar historico temporal com data de criacao quando chamar criar")]
    [Trait("ValueObject", "HistoricoTemporal")]
    public void Criar_DeveCriar_QuandoChamado()
    {
        // Arrange
        var antes = DateTimeOffset.UtcNow;

        // Act
        var historico = HistoricoTemporal.Criar();

        // Assert
        historico.DataCriacao.ShouldBeGreaterThanOrEqualTo(antes);
        historico.DataInicioProcessamento.ShouldBeNull();
        historico.DataConclusaoProcessamento.ShouldBeNull();
    }

    [Fact(DisplayName = "Deve criar historico temporal com data informada")]
    [Trait("ValueObject", "HistoricoTemporal")]
    public void Criar_DeveCriar_QuandoDataInformada()
    {
        // Arrange
        var dataCriacao = DateTimeOffset.UtcNow.AddMinutes(-5);

        // Act
        var historico = HistoricoTemporal.Criar(dataCriacao);

        // Assert
        historico.DataCriacao.ShouldBe(dataCriacao);
    }

    [Fact(DisplayName = "Deve lancar excecao ao reidratar com data de criacao default")]
    [Trait("ValueObject", "HistoricoTemporal")]
    public void Reidratar_DeveLancarExcecao_QuandoDataCriacaoDefault()
    {
        // Arrange
        var dataCriacao = default(DateTimeOffset);

        // Act
        Action acao = () => _ = HistoricoTemporal.Reidratar(dataCriacao, null, null);

        // Assert
        var excecao = Should.Throw<DomainException>(acao);
        excecao.ErrorType.ShouldBe(ErrorType.InvalidInput);
        excecao.Message.ShouldContain("data de criação é obrigatória");
    }

    [Fact(DisplayName = "Deve marcar inicio do processamento")]
    [Trait("ValueObject", "HistoricoTemporal")]
    public void MarcarInicioProcessamento_DeveDefinirData_QuandoChamado()
    {
        // Arrange
        var historico = HistoricoTemporal.Criar(DateTimeOffset.UtcNow.AddMinutes(-10));
        var dataInicio = DateTimeOffset.UtcNow.AddMinutes(-2);

        // Act
        var atualizado = historico.MarcarInicioProcessamento(dataInicio);

        // Assert
        atualizado.DataInicioProcessamento.ShouldBe(dataInicio);
    }

    [Fact(DisplayName = "Deve marcar conclusao do processamento")]
    [Trait("ValueObject", "HistoricoTemporal")]
    public void MarcarConclusaoProcessamento_DeveDefinirData_QuandoChamado()
    {
        // Arrange
        var historico = HistoricoTemporal.Criar(DateTimeOffset.UtcNow.AddMinutes(-10)).MarcarInicioProcessamento(DateTimeOffset.UtcNow.AddMinutes(-5));
        var dataConclusao = DateTimeOffset.UtcNow.AddMinutes(-1);

        // Act
        var atualizado = historico.MarcarConclusaoProcessamento(dataConclusao);

        // Assert
        atualizado.DataConclusaoProcessamento.ShouldBe(dataConclusao);
    }

    [Fact(DisplayName = "Deve lancar excecao quando inicio for anterior a criacao")]
    [Trait("ValueObject", "HistoricoTemporal")]
    public void MarcarInicioProcessamento_DeveLancarExcecao_QuandoAnteriorACriacao()
    {
        // Arrange
        var dataCriacao = DateTimeOffset.UtcNow;
        var historico = HistoricoTemporal.Criar(dataCriacao);

        // Act
        Action acao = () => _ = historico.MarcarInicioProcessamento(dataCriacao.AddSeconds(-1));

        // Assert
        var excecao = Should.Throw<DomainException>(acao);
        excecao.ErrorType.ShouldBe(ErrorType.DomainRuleBroken);
        excecao.Message.ShouldContain("não pode ser anterior à data de criação");
    }

    [Fact(DisplayName = "Deve lancar excecao quando conclusao for anterior ao inicio")]
    [Trait("ValueObject", "HistoricoTemporal")]
    public void MarcarConclusaoProcessamento_DeveLancarExcecao_QuandoAnteriorAInicio()
    {
        // Arrange
        var dataCriacao = DateTimeOffset.UtcNow.AddMinutes(-2);
        var dataInicio = DateTimeOffset.UtcNow.AddMinutes(-1);
        var historico = HistoricoTemporal.Criar(dataCriacao).MarcarInicioProcessamento(dataInicio);

        // Act
        Action acao = () => _ = historico.MarcarConclusaoProcessamento(dataInicio.AddSeconds(-1));

        // Assert
        var excecao = Should.Throw<DomainException>(acao);
        excecao.ErrorType.ShouldBe(ErrorType.DomainRuleBroken);
        excecao.Message.ShouldContain("não pode ser anterior à data de início");
    }

    [Fact(DisplayName = "Deve reidratar historico temporal quando datas validas")]
    [Trait("ValueObject", "HistoricoTemporal")]
    public void Reidratar_DeveCriar_QuandoDatasValidas()
    {
        // Arrange
        var dataCriacao = DateTimeOffset.UtcNow.AddMinutes(-10);
        var dataInicio = DateTimeOffset.UtcNow.AddMinutes(-8);
        var dataConclusao = DateTimeOffset.UtcNow.AddMinutes(-4);

        // Act
        var historico = HistoricoTemporal.Reidratar(dataCriacao, dataInicio, dataConclusao);

        // Assert
        historico.DataCriacao.ShouldBe(dataCriacao);
        historico.DataInicioProcessamento.ShouldBe(dataInicio);
        historico.DataConclusaoProcessamento.ShouldBe(dataConclusao);
    }
}
