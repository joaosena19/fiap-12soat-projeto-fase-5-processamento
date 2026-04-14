namespace Tests.Domain.ProcessamentoDiagrama;

public class ProcessamentoDiagramaAggregateTests
{
    #region Criar

    [Fact(DisplayName = "Deve criar processamento com status aguardando")]
    [Trait("Entity", "ProcessamentoDiagrama")]
    public void Criar_DeveCriarComSucesso_QuandoAnaliseDiagramaIdValido()
    {
        // Arrange
        var analiseDiagramaId = Guid.NewGuid();

        // Act
        var processamento = ProcessamentoDiagramaAggregate.Criar(analiseDiagramaId);

        // Assert
        processamento.DeveEstarRecentementeCriado(analiseDiagramaId);
    }

    [Fact(DisplayName = "Deve lancar excecao quando analise diagrama id for vazio")]
    [Trait("Entity", "ProcessamentoDiagrama")]
    public void Criar_DeveLancarExcecao_QuandoAnaliseDiagramaIdVazio()
    {
        // Arrange
        var analiseDiagramaId = Guid.Empty;

        // Act
        Action acao = () => _ = ProcessamentoDiagramaAggregate.Criar(analiseDiagramaId);

        // Assert
        var excecao = Should.Throw<DomainException>(acao);
        excecao.Message.ShouldContain("AnaliseDiagramaId não pode ser vazio");
    }

    #endregion

    #region IniciarProcessamento

    [Fact(DisplayName = "Deve mudar status para em processamento quando iniciar")]
    [Trait("Entity", "ProcessamentoDiagrama")]
    public void IniciarProcessamento_DeveMudarStatus_QuandoAguardando()
    {
        // Arrange
        var processamento = new ProcessamentoDiagramaBuilder().Build();

        // Act
        processamento.IniciarProcessamento();

        // Assert
        processamento.DeveEstarComProcessamentoIniciado();
    }

    [Fact(DisplayName = "Deve permitir iniciar processamento quando status for falha")]
    [Trait("Entity", "ProcessamentoDiagrama")]
    public void IniciarProcessamento_DevePermitir_QuandoStatusFalha()
    {
        // Arrange
        var processamento = new ProcessamentoDiagramaBuilder().ComFalha(2).Build();

        // Act
        processamento.IniciarProcessamento();

        // Assert
        processamento.DeveEstarComStatus(StatusProcessamentoEnum.EmProcessamento);
    }

    [Fact(DisplayName = "Deve lancar excecao ao iniciar processamento quando ja em processamento")]
    [Trait("Entity", "ProcessamentoDiagrama")]
    public void IniciarProcessamento_DeveLancarExcecao_QuandoJaEmProcessamento()
    {
        // Arrange
        var processamento = new ProcessamentoDiagramaBuilder().EmProcessamento().Build();

        // Act
        Action acao = () => processamento.IniciarProcessamento();

        // Assert
        acao.DeveLancarExcecaoDeValidacao("Só é possível iniciar processamento");
    }

    [Fact(DisplayName = "Deve lancar excecao ao iniciar processamento quando concluido")]
    [Trait("Entity", "ProcessamentoDiagrama")]
    public void IniciarProcessamento_DeveLancarExcecao_QuandoConcluido()
    {
        // Arrange
        var processamento = new ProcessamentoDiagramaBuilder().Concluido().Build();

        // Act
        Action acao = () => processamento.IniciarProcessamento();

        // Assert
        acao.DeveLancarExcecaoDeValidacao("Só é possível iniciar processamento");
    }

    #endregion

    #region ConcluirProcessamento

    [Fact(DisplayName = "Deve concluir processamento quando em processamento")]
    [Trait("Entity", "ProcessamentoDiagrama")]
    public void ConcluirProcessamento_DeveConcluir_QuandoStatusEmProcessamento()
    {
        // Arrange
        var processamento = new ProcessamentoDiagramaBuilder().EmProcessamento().Build();

        // Act
        processamento.ConcluirProcessamento("Descricao da analise", ["API Gateway"], ["Ponto unico de falha"], ["Implementar redundancia"], 2);

        // Assert
        processamento.DeveEstarConcluido(2);
    }

    [Fact(DisplayName = "Deve lancar excecao ao concluir processamento quando nao em processamento")]
    [Trait("Entity", "ProcessamentoDiagrama")]
    public void ConcluirProcessamento_DeveLancarExcecao_QuandoNaoEmProcessamento()
    {
        // Arrange
        var processamento = new ProcessamentoDiagramaBuilder().Build();

        // Act
        Action acao = () => processamento.ConcluirProcessamento("Descricao da analise", ["API Gateway"], ["Ponto unico de falha"], ["Implementar redundancia"], 1);

        // Assert
        acao.DeveLancarExcecaoDeValidacao("Só é possível concluir processamento");
    }

    #endregion

    #region RegistrarFalha

    [Fact(DisplayName = "Deve registrar falha quando em processamento")]
    [Trait("Entity", "ProcessamentoDiagrama")]
    public void RegistrarFalha_DeveRegistrar_QuandoStatusEmProcessamento()
    {
        // Arrange
        var processamento = new ProcessamentoDiagramaBuilder().EmProcessamento().Build();

        // Act
        processamento.RegistrarFalha(3);

        // Assert
        processamento.DeveEstarComFalha(3);
    }

    [Fact(DisplayName = "Deve lancar excecao ao registrar falha quando nao em processamento")]
    [Trait("Entity", "ProcessamentoDiagrama")]
    public void RegistrarFalha_DeveLancarExcecao_QuandoNaoEmProcessamento()
    {
        // Arrange
        var processamento = new ProcessamentoDiagramaBuilder().Build();

        // Act
        Action acao = () => processamento.RegistrarFalha(2);

        // Assert
        acao.DeveLancarExcecaoDeValidacao("Só é possível registrar falha");
    }

    #endregion

    #region RegistrarRejeicao

    [Fact(DisplayName = "Deve registrar rejeicao quando em processamento")]
    [Trait("Entity", "ProcessamentoDiagrama")]
    public void RegistrarRejeicao_DeveRegistrar_QuandoStatusEmProcessamento()
    {
        // Arrange
        var processamento = new ProcessamentoDiagramaBuilder().EmProcessamento().Build();

        // Act
        processamento.RegistrarRejeicao(1);

        // Assert
        processamento.DeveEstarRejeitado(1);
    }

    [Fact(DisplayName = "Deve lancar excecao ao registrar rejeicao quando nao em processamento")]
    [Trait("Entity", "ProcessamentoDiagrama")]
    public void RegistrarRejeicao_DeveLancarExcecao_QuandoNaoEmProcessamento()
    {
        // Arrange
        var processamento = new ProcessamentoDiagramaBuilder().Build();

        // Act
        Action acao = () => processamento.RegistrarRejeicao(1);

        // Assert
        acao.DeveLancarExcecaoDeValidacao("Só é possível registrar rejeição");
    }

    [Fact(DisplayName = "Deve lancar excecao ao iniciar processamento quando rejeitado")]
    [Trait("Entity", "ProcessamentoDiagrama")]
    public void IniciarProcessamento_DeveLancarExcecao_QuandoRejeitado()
    {
        // Arrange
        var processamento = new ProcessamentoDiagramaBuilder().Rejeitado().Build();

        // Act
        Action acao = () => processamento.IniciarProcessamento();

        // Assert
        acao.DeveLancarExcecaoDeValidacao("Só é possível iniciar processamento");
    }

    #endregion

    #region RegistrarDadosOrigem

    [Fact(DisplayName = "Deve registrar dados de origem quando valores validos")]
    [Trait("Entity", "ProcessamentoDiagrama")]
    public void RegistrarDadosOrigem_DeveRegistrar_QuandoValoresValidos()
    {
        // Arrange
        var processamento = new ProcessamentoDiagramaBuilder().Build();

        // Act
        processamento.RegistrarDadosOrigem("s3://bucket/arquivo.png", "arquivo.png", "original.png", "png");

        // Assert
        processamento.DadosOrigem.ShouldNotBeNull();
        processamento.DadosOrigem.LocalizacaoUrl.Valor.ShouldBe("s3://bucket/arquivo.png");
        processamento.DadosOrigem.NomeFisico.Valor.ShouldBe("arquivo.png");
        processamento.DadosOrigem.NomeOriginal.Valor.ShouldBe("original.png");
        processamento.DadosOrigem.Extensao.Valor.ShouldBe("png");
    }

    [Fact(DisplayName = "Deve lancar excecao ao registrar dados de origem com localizacao url vazia")]
    [Trait("Entity", "ProcessamentoDiagrama")]
    public void RegistrarDadosOrigem_DeveLancarExcecao_QuandoLocalizacaoUrlVazia()
    {
        // Arrange
        var processamento = new ProcessamentoDiagramaBuilder().Build();

        // Act
        Action acao = () => processamento.RegistrarDadosOrigem("", "arquivo.png", "original.png", "png");

        // Assert
        acao.DeveLancarExcecaoDeValidacao("Localização URL não pode ser vazia");
    }

    [Fact(DisplayName = "Deve permitir sobrescrever dados de origem existentes")]
    [Trait("Entity", "ProcessamentoDiagrama")]
    public void RegistrarDadosOrigem_DeveSobrescrever_QuandoJaExiste()
    {
        // Arrange
        var processamento = new ProcessamentoDiagramaBuilder().ComDadosOrigem().Build();

        // Act
        processamento.RegistrarDadosOrigem("s3://bucket/novo-arquivo.png", "novo.png", "novo-original.png", "pdf");

        // Assert
        processamento.DadosOrigem!.LocalizacaoUrl.Valor.ShouldBe("s3://bucket/novo-arquivo.png");
        processamento.DadosOrigem.NomeFisico.Valor.ShouldBe("novo.png");
    }

    [Fact(DisplayName = "Deve iniciar processamento sem dados de origem preenchidos")]
    [Trait("Entity", "ProcessamentoDiagrama")]
    public void DadosOrigem_DeveSerNulo_QuandoNaoRegistrado()
    {
        // Arrange & Act
        var processamento = new ProcessamentoDiagramaBuilder().Build();

        // Assert
        processamento.DadosOrigem.ShouldBeNull();
    }

    #endregion
}
