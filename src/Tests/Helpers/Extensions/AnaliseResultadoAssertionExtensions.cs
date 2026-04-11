using Domain.ProcessamentoDiagrama.Entities;

namespace Tests.Helpers.Extensions;

public static class AnaliseResultadoAssertionExtensions
{
    public static void DeveConterContagens(this AnaliseResultado analise, string descricao, int componentes, int riscos, int recomendacoes)
    {
        analise.DescricaoAnalise.Valor.ShouldBe(descricao);
        analise.ComponentesIdentificados.Count.ShouldBe(componentes);
        analise.RiscosArquiteturais.Count.ShouldBe(riscos);
        analise.RecomendacoesBasicas.Count.ShouldBe(recomendacoes);
    }

    public static void DeveConterComponentes(this AnaliseResultado analise, params string[] componentesEsperados)
    {
        analise.ComponentesIdentificados.Select(item => item.Valor).ShouldBe(componentesEsperados);
    }

    public static void DeveConterRiscos(this AnaliseResultado analise, params string[] riscosEsperados)
    {
        analise.RiscosArquiteturais.Select(item => item.Valor).ShouldBe(riscosEsperados);
    }

    public static void DeveConterRecomendacoes(this AnaliseResultado analise, params string[] recomendacoesEsperadas)
    {
        analise.RecomendacoesBasicas.Select(item => item.Valor).ShouldBe(recomendacoesEsperadas);
    }
}
