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
}
