namespace Tests.Helpers.Extensions;

public static class ProcessamentoDiagramaAssertionExtensions
{
    public static void DeveEstarComStatus(this ProcessamentoDiagramaAggregate aggregate, StatusProcessamentoEnum statusEsperado)
    {
        aggregate.StatusProcessamento.Valor.ShouldBe(statusEsperado);
    }

    public static void DeveConterAnaliseResultado(this ProcessamentoDiagramaAggregate aggregate)
    {
        aggregate.AnaliseResultado.ShouldNotBeNull();
    }

    public static void NaoDeveConterAnaliseResultado(this ProcessamentoDiagramaAggregate aggregate)
    {
        aggregate.AnaliseResultado.ShouldBeNull();
    }

    public static void DeveEstarRecentementeCriado(this ProcessamentoDiagramaAggregate aggregate, Guid analiseDiagramaId)
    {
        aggregate.Id.ShouldNotBe(Guid.Empty);
        aggregate.AnaliseDiagramaId.ShouldBe(analiseDiagramaId);
        aggregate.DeveEstarComStatus(StatusProcessamentoEnum.AguardandoProcessamento);
        aggregate.TentativasProcessamento.Valor.ShouldBe(0);
        aggregate.NaoDeveConterAnaliseResultado();
        aggregate.HistoricoTemporal.DataCriacao.ShouldNotBe(default);
    }

    public static void DeveEstarComProcessamentoIniciado(this ProcessamentoDiagramaAggregate aggregate)
    {
        aggregate.DeveEstarComStatus(StatusProcessamentoEnum.EmProcessamento);
        aggregate.HistoricoTemporal.DataInicioProcessamento.ShouldNotBeNull();
    }

    public static void DeveEstarConcluido(this ProcessamentoDiagramaAggregate aggregate, int tentativasEsperadas)
    {
        aggregate.DeveEstarComStatus(StatusProcessamentoEnum.Concluido);
        aggregate.DeveConterAnaliseResultado();
        aggregate.TentativasProcessamento.Valor.ShouldBe(tentativasEsperadas);
        aggregate.HistoricoTemporal.DataConclusaoProcessamento.ShouldNotBeNull();
    }

    public static void DeveEstarComFalha(this ProcessamentoDiagramaAggregate aggregate, int tentativasEsperadas)
    {
        aggregate.DeveEstarComStatus(StatusProcessamentoEnum.Falha);
        aggregate.TentativasProcessamento.Valor.ShouldBe(tentativasEsperadas);
        aggregate.NaoDeveConterAnaliseResultado();
        aggregate.HistoricoTemporal.DataConclusaoProcessamento.ShouldNotBeNull();
    }

    public static void DeveEstarRejeitado(this ProcessamentoDiagramaAggregate aggregate, int tentativasEsperadas)
    {
        aggregate.DeveEstarComStatus(StatusProcessamentoEnum.Rejeitado);
        aggregate.TentativasProcessamento.Valor.ShouldBe(tentativasEsperadas);
        aggregate.NaoDeveConterAnaliseResultado();
        aggregate.HistoricoTemporal.DataConclusaoProcessamento.ShouldNotBeNull();
    }

    public static void DeveConterDadosOrigem(this ProcessamentoDiagramaAggregate aggregate)
    {
        aggregate.DadosOrigem.ShouldNotBeNull();
    }

    public static void NaoDeveConterDadosOrigem(this ProcessamentoDiagramaAggregate aggregate)
    {
        aggregate.DadosOrigem.ShouldBeNull();
    }
}
