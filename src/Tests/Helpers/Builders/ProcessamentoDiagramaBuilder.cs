using Domain.ProcessamentoDiagrama.Aggregates;

namespace Tests.Helpers.Builders;

public class ProcessamentoDiagramaBuilder
{
    private Guid _analiseDiagramaId = Guid.NewGuid();
    private bool _emProcessamento;
    private bool _concluido;
    private bool _falha;
    private bool _rejeitado;
    private int _tentativasFalha = 1;
    private bool _comDadosOrigem;
    private string _localizacaoUrl = "s3://bucket/diagrama-123.png";
    private string _nomeFisico = "diagrama-123.png";
    private string _nomeOriginal = "diagrama-arquitetura.png";
    private string _extensao = "png";

    public ProcessamentoDiagramaBuilder ComAnaliseDiagramaId(Guid analiseDiagramaId)
    {
        _analiseDiagramaId = analiseDiagramaId;
        return this;
    }

    public ProcessamentoDiagramaBuilder EmProcessamento()
    {
        _emProcessamento = true;
        _concluido = false;
        _falha = false;
        return this;
    }

    public ProcessamentoDiagramaBuilder Concluido()
    {
        _concluido = true;
        _emProcessamento = false;
        _falha = false;
        return this;
    }

    public ProcessamentoDiagramaBuilder ComFalha(int tentativas = 1)
    {
        _falha = true;
        _tentativasFalha = tentativas;
        _emProcessamento = false;
        _concluido = false;
        _rejeitado = false;
        return this;
    }

    public ProcessamentoDiagramaBuilder Rejeitado(int tentativas = 1)
    {
        _rejeitado = true;
        _tentativasFalha = tentativas;
        _emProcessamento = false;
        _concluido = false;
        _falha = false;
        return this;
    }

    public ProcessamentoDiagramaBuilder ComDadosOrigem(string? localizacaoUrl = null, string? nomeFisico = null, string? nomeOriginal = null, string? extensao = null)
    {
        _comDadosOrigem = true;
        if (localizacaoUrl != null) _localizacaoUrl = localizacaoUrl;
        if (nomeFisico != null) _nomeFisico = nomeFisico;
        if (nomeOriginal != null) _nomeOriginal = nomeOriginal;
        if (extensao != null) _extensao = extensao;
        return this;
    }

    public ProcessamentoDiagrama Build()
    {
        var processamento = ProcessamentoDiagrama.Criar(_analiseDiagramaId);

        if (_comDadosOrigem)
            processamento.RegistrarDadosOrigem(_localizacaoUrl, _nomeFisico, _nomeOriginal, _extensao);

        if (_concluido)
        {
            processamento.IniciarProcessamento();
            processamento.ConcluirProcessamento(
                "Analise concluida",
                ["API Gateway"],
                ["Ponto unico de falha"],
                ["Implementar circuit breaker"],
                1);
            return processamento;
        }

        if (_falha)
        {
            processamento.IniciarProcessamento();
            processamento.RegistrarFalha(_tentativasFalha);
            return processamento;
        }

        if (_rejeitado)
        {
            processamento.IniciarProcessamento();
            processamento.RegistrarRejeicao(_tentativasFalha);
            return processamento;
        }

        if (_emProcessamento)
            processamento.IniciarProcessamento();

        return processamento;
    }
}
