using Domain.ProcessamentoDiagrama.Aggregates;

namespace Tests.Helpers.Builders;

public class ProcessamentoDiagramaBuilder
{
    private Guid _analiseDiagramaId = Guid.NewGuid();
    private bool _emProcessamento;
    private bool _concluido;
    private bool _falha;
    private int _tentativasFalha = 1;

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
        return this;
    }

    public ProcessamentoDiagrama Build()
    {
        var processamento = ProcessamentoDiagrama.Criar(_analiseDiagramaId);

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

        if (_emProcessamento)
            processamento.IniciarProcessamento();

        return processamento;
    }
}
