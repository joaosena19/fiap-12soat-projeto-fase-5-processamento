using Domain.ProcessamentoDiagrama.Entities;

namespace Tests.Helpers.Builders;

public class AnaliseResultadoBuilder
{
    private string _descricao = "Analise completa";
    private List<string> _componentes = ["API Gateway", "Servico A"];
    private List<string> _riscos = ["Ponto unico de falha"];
    private List<string> _recomendacoes = ["Adicionar redundancia"];

    public AnaliseResultadoBuilder ComDescricao(string descricao) { _descricao = descricao; return this; }
    public AnaliseResultadoBuilder ComComponentes(params string[] componentes) { _componentes = [..componentes]; return this; }
    public AnaliseResultadoBuilder ComRiscos(params string[] riscos) { _riscos = [..riscos]; return this; }
    public AnaliseResultadoBuilder ComRecomendacoes(params string[] recomendacoes) { _recomendacoes = [..recomendacoes]; return this; }

    public AnaliseResultado Build() => AnaliseResultado.Criar(_descricao, _componentes, _riscos, _recomendacoes);
}
