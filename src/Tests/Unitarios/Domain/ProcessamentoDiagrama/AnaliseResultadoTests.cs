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
        analise.DeveConterContagens(descricao, 2, 1, 1);
    }

    [Fact(DisplayName = "Deve mapear componentes identificados corretamente")]
    [Trait("Entity", "AnaliseResultado")]
    public void Criar_DeveMapearComponentes_QuandoListaValida()
    {
        // Arrange & Act
        var analise = new AnaliseResultadoBuilder().ComComponentes("Componente 1", "Componente 2").Build();

        // Assert
        analise.DeveConterComponentes("Componente 1", "Componente 2");
    }

    [Fact(DisplayName = "Deve mapear riscos arquiteturais corretamente")]
    [Trait("Entity", "AnaliseResultado")]
    public void Criar_DeveMapearRiscos_QuandoListaValida()
    {
        // Arrange & Act
        var analise = new AnaliseResultadoBuilder().ComRiscos("Risco 1", "Risco 2").Build();

        // Assert
        analise.DeveConterRiscos("Risco 1", "Risco 2");
    }

    [Fact(DisplayName = "Deve mapear recomendacoes basicas corretamente")]
    [Trait("Entity", "AnaliseResultado")]
    public void Criar_DeveMapearRecomendacoes_QuandoListaValida()
    {
        // Arrange & Act
        var analise = new AnaliseResultadoBuilder().ComRecomendacoes("Recomendacao 1", "Recomendacao 2").Build();

        // Assert
        analise.DeveConterRecomendacoes("Recomendacao 1", "Recomendacao 2");
    }

    [Fact(DisplayName = "Deve lancar excecao quando descricao for vazia")]
    [Trait("Entity", "AnaliseResultado")]
    public void Criar_DeveLancarExcecao_QuandoDescricaoVazia()
    {
        // Arrange
        Action acao = () => _ = AnaliseResultado.Criar("", new List<string> { "Componente" }, new List<string> { "Risco" }, new List<string> { "Recomendacao" });

        // Act & Assert
        acao.DeveLancarExcecaoDeValidacao("Descrição da análise não pode ser vazia");
    }

    [Fact(DisplayName = "Deve lancar excecao quando lista contem componente vazio")]
    [Trait("Entity", "AnaliseResultado")]
    public void Criar_DeveLancarExcecao_QuandoComponenteVazioNaLista()
    {
        // Arrange
        Action acao = () => _ = AnaliseResultado.Criar("Descricao", new List<string> { "Componente 1", " " }, new List<string> { "Risco" }, new List<string> { "Recomendacao" });

        // Act & Assert
        acao.DeveLancarExcecaoDeValidacao("Componente identificado não pode ser vazio");
    }
}
