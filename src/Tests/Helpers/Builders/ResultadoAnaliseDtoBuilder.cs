namespace Tests.Helpers.Builders;

public class ResultadoAnaliseDtoBuilder
{
    private bool _sucesso = true;
    private string? _descricaoAnalise = "Analise do diagrama de arquitetura";
    private List<string> _componentesIdentificados = ["API Gateway", "Servico de Pagamento"];
    private List<string> _riscosArquiteturais = ["Ponto unico de falha no API Gateway"];
    private List<string> _recomendacoesBasicas = ["Implementar circuit breaker"];
    private string? _motivoErro;
    private int _tentativasRealizadas = 1;

    public ResultadoAnaliseDtoBuilder Sucesso()
    {
        _sucesso = true;
        _descricaoAnalise = "Analise do diagrama de arquitetura";
        _motivoErro = null;
        return this;
    }

    public ResultadoAnaliseDtoBuilder ComFalha(string motivo, int tentativas = 1)
    {
        _sucesso = false;
        _descricaoAnalise = null;
        _motivoErro = motivo;
        _tentativasRealizadas = tentativas;
        return this;
    }

    public ResultadoAnaliseDtoBuilder ComFalhaSemMotivo(int tentativas = 1)
    {
        _sucesso = false;
        _descricaoAnalise = null;
        _motivoErro = null;
        _tentativasRealizadas = tentativas;
        _componentesIdentificados = [];
        _riscosArquiteturais = [];
        _recomendacoesBasicas = [];
        return this;
    }

    public ResultadoAnaliseDtoBuilder ComTentativas(int tentativas)
    {
        _tentativasRealizadas = tentativas;
        return this;
    }

    public ResultadoAnaliseDto Build()
    {
        return new ResultadoAnaliseDto
        {
            Sucesso = _sucesso,
            DescricaoAnalise = _descricaoAnalise,
            ComponentesIdentificados = _componentesIdentificados,
            RiscosArquiteturais = _riscosArquiteturais,
            RecomendacoesBasicas = _recomendacoesBasicas,
            MotivoErro = _motivoErro,
            TentativasRealizadas = _tentativasRealizadas
        };
    }
}
