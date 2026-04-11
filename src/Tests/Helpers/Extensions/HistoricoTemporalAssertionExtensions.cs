namespace Tests.Helpers.Extensions;

public static class HistoricoTemporalAssertionExtensions
{
    public static void DeveConterDatas(this HistoricoTemporal historico, DateTimeOffset criacao, DateTimeOffset? inicio, DateTimeOffset? conclusao)
    {
        historico.DataCriacao.ShouldBe(criacao);
        historico.DataInicioProcessamento.ShouldBe(inicio);
        historico.DataConclusaoProcessamento.ShouldBe(conclusao);
    }

    public static void DeveEstarRecemCriado(this HistoricoTemporal historico, DateTimeOffset anteriorA)
    {
        historico.DataCriacao.ShouldBeGreaterThanOrEqualTo(anteriorA);
        historico.DataInicioProcessamento.ShouldBeNull();
        historico.DataConclusaoProcessamento.ShouldBeNull();
    }

    public static void DeveConterDataCriacao(this HistoricoTemporal historico, DateTimeOffset dataEsperada)
    {
        historico.DataCriacao.ShouldBe(dataEsperada);
    }

    public static void DeveConterDataInicio(this HistoricoTemporal historico, DateTimeOffset dataEsperada)
    {
        historico.DataInicioProcessamento.ShouldBe(dataEsperada);
    }

    public static void DeveConterDataConclusao(this HistoricoTemporal historico, DateTimeOffset dataEsperada)
    {
        historico.DataConclusaoProcessamento.ShouldBe(dataEsperada);
    }
}
