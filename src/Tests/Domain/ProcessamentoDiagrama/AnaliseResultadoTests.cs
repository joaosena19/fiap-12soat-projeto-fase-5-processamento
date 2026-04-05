using Domain.ProcessamentoDiagrama.Entities;

namespace Tests.Domain.ProcessamentoDiagrama;

public class AnaliseResultadoTests
{
    [Fact(DisplayName = "Deve criar analise resultado quando dados validos")]
    [Trait("Entity", "AnaliseResultado")]
    public void Criar_DeveCriar_QuandoDadosValidos()
    {
        // Arrange
        var descricao = "Analise completa";
        var componentes = new List<string> { "API Gateway", "Servico A" };
        var riscos = new List<string> { "Ponto unico de falha" };
        var recomendacoes = new List<string> { "Adicionar redundancia" };

        // Act
        var analise = AnaliseResultado.Criar(descricao, componentes, riscos, recomendacoes);

        // Assert
        analise.DescricaoAnalise.Valor.ShouldBe(descricao);
        analise.ComponentesIdentificados.Count.ShouldBe(2);
        analise.RiscosArquiteturais.Count.ShouldBe(1);
        analise.RecomendacoesBasicas.Count.ShouldBe(1);
    }

    [Fact(DisplayName = "Deve mapear componentes identificados corretamente")]
    [Trait("Entity", "AnaliseResultado")]
    public void Criar_DeveMapearComponentes_QuandoListaValida()
    {
        // Arrange
        var componentes = new List<string> { "Componente 1", "Componente 2" };

        // Act
        var analise = AnaliseResultado.Criar("Descricao", componentes, new List<string> { "Risco" }, new List<string> { "Recomendacao" });

        // Assert
        analise.ComponentesIdentificados.Select(item => item.Valor).ShouldBe(componentes);
    }

    [Fact(DisplayName = "Deve mapear riscos arquiteturais corretamente")]
    [Trait("Entity", "AnaliseResultado")]
    public void Criar_DeveMapearRiscos_QuandoListaValida()
    {
        // Arrange
        var riscos = new List<string> { "Risco 1", "Risco 2" };

        // Act
        var analise = AnaliseResultado.Criar("Descricao", new List<string> { "Componente" }, riscos, new List<string> { "Recomendacao" });

        // Assert
        analise.RiscosArquiteturais.Select(item => item.Valor).ShouldBe(riscos);
    }

    [Fact(DisplayName = "Deve mapear recomendacoes basicas corretamente")]
    [Trait("Entity", "AnaliseResultado")]
    public void Criar_DeveMapearRecomendacoes_QuandoListaValida()
    {
        // Arrange
        var recomendacoes = new List<string> { "Recomendacao 1", "Recomendacao 2" };

        // Act
        var analise = AnaliseResultado.Criar("Descricao", new List<string> { "Componente" }, new List<string> { "Risco" }, recomendacoes);

        // Assert
        analise.RecomendacoesBasicas.Select(item => item.Valor).ShouldBe(recomendacoes);
    }

    [Fact(DisplayName = "Deve lancar excecao quando descricao for vazia")]
    [Trait("Entity", "AnaliseResultado")]
    public void Criar_DeveLancarExcecao_QuandoDescricaoVazia()
    {
        // Arrange
        Action acao = () => _ = AnaliseResultado.Criar("", new List<string> { "Componente" }, new List<string> { "Risco" }, new List<string> { "Recomendacao" });

        // Act
        var excecao = Should.Throw<DomainException>(acao);

        // Assert
        excecao.ErrorType.ShouldBe(ErrorType.InvalidInput);
        excecao.Message.ShouldContain("Descrição da análise não pode ser vazia");
    }

    [Fact(DisplayName = "Deve lancar excecao quando lista contem componente vazio")]
    [Trait("Entity", "AnaliseResultado")]
    public void Criar_DeveLancarExcecao_QuandoComponenteVazioNaLista()
    {
        // Arrange
        Action acao = () => _ = AnaliseResultado.Criar("Descricao", new List<string> { "Componente 1", " " }, new List<string> { "Risco" }, new List<string> { "Recomendacao" });

        // Act
        var excecao = Should.Throw<DomainException>(acao);

        // Assert
        excecao.ErrorType.ShouldBe(ErrorType.InvalidInput);
        excecao.Message.ShouldContain("Componente identificado não pode ser vazio");
    }
}
