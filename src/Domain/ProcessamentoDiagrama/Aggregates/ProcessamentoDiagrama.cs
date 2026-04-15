using Domain.ProcessamentoDiagrama.Entities;
using Domain.ProcessamentoDiagrama.Enums;
using Domain.ProcessamentoDiagrama.ValueObjects;
using Shared.Attributes;
using Shared.Exceptions;
using UUIDNext;

namespace Domain.ProcessamentoDiagrama.Aggregates;

[AggregateRoot]
public class ProcessamentoDiagrama
{
    public Guid Id { get; private set; }
    public Guid AnaliseDiagramaId { get; private set; }
    public StatusProcessamento StatusProcessamento { get; private set; } = null!;
    public TentativasProcessamento TentativasProcessamento { get; private set; } = null!;
    public AnaliseResultado? AnaliseResultado { get; private set; }
    public DadosOrigem? DadosOrigem { get; private set; }
    public HistoricoTemporal HistoricoTemporal { get; private set; } = null!;

    // Construtor sem parâmetro para EF Core
    private ProcessamentoDiagrama() { }

    private ProcessamentoDiagrama(Guid id, Guid analiseDiagramaId, StatusProcessamento statusProcessamento, TentativasProcessamento tentativasProcessamento, HistoricoTemporal historicoTemporal, AnaliseResultado? analiseResultado)
    {
        Id = id;
        AnaliseDiagramaId = analiseDiagramaId;
        StatusProcessamento = statusProcessamento;
        TentativasProcessamento = tentativasProcessamento;
        HistoricoTemporal = historicoTemporal;
        AnaliseResultado = analiseResultado;
    }

    public static ProcessamentoDiagrama Criar(Guid analiseDiagramaId)
    {
        if (analiseDiagramaId == Guid.Empty)
            throw new DomainException("AnaliseDiagramaId não pode ser vazio");

        return new ProcessamentoDiagrama(
            Uuid.NewSequential(),
            analiseDiagramaId,
            new StatusProcessamento(StatusProcessamentoEnum.AguardandoProcessamento),
            new TentativasProcessamento(0),
            HistoricoTemporal.Criar(),
            null);
    }

    public void IniciarProcessamento()
    {
        if (StatusProcessamento.Valor != StatusProcessamentoEnum.AguardandoProcessamento && StatusProcessamento.Valor != StatusProcessamentoEnum.Falha)
            throw new DomainException("Só é possível iniciar processamento quando o status é AguardandoProcessamento ou Falha");

        StatusProcessamento = new StatusProcessamento(StatusProcessamentoEnum.EmProcessamento);
        HistoricoTemporal = HistoricoTemporal.MarcarInicioProcessamento();
    }

    public void ConcluirProcessamento(string descricaoAnalise, List<string> componentesIdentificados, List<string> riscosArquiteturais, List<string> recomendacoesBasicas, int tentativasRealizadas)
    {
        if (StatusProcessamento.Valor != StatusProcessamentoEnum.EmProcessamento)
            throw new DomainException("Só é possível concluir processamento quando o status é EmProcessamento");

        AnaliseResultado = AnaliseResultado.Criar(descricaoAnalise, componentesIdentificados, riscosArquiteturais, recomendacoesBasicas);
        TentativasProcessamento = new TentativasProcessamento(tentativasRealizadas);
        StatusProcessamento = new StatusProcessamento(StatusProcessamentoEnum.Concluido);
        HistoricoTemporal = HistoricoTemporal.MarcarConclusaoProcessamento();
    }

    public void RegistrarFalha(int tentativasRealizadas)
    {
        if (StatusProcessamento.Valor != StatusProcessamentoEnum.EmProcessamento)
            throw new DomainException("Só é possível registrar falha quando o status é EmProcessamento");

        TentativasProcessamento = new TentativasProcessamento(tentativasRealizadas);
        StatusProcessamento = new StatusProcessamento(StatusProcessamentoEnum.Falha);
        HistoricoTemporal = HistoricoTemporal.MarcarConclusaoProcessamento();
    }

    public void RegistrarRejeicao(int tentativasRealizadas)
    {
        if (StatusProcessamento.Valor != StatusProcessamentoEnum.EmProcessamento)
            throw new DomainException("Só é possível registrar rejeição quando o status é EmProcessamento");

        TentativasProcessamento = new TentativasProcessamento(tentativasRealizadas);
        StatusProcessamento = new StatusProcessamento(StatusProcessamentoEnum.Rejeitado);
        HistoricoTemporal = HistoricoTemporal.MarcarConclusaoProcessamento();
    }

    public void RegistrarDadosOrigem(string localizacaoUrl, string nomeFisico, string nomeOriginal, string extensao)
    {
        DadosOrigem = DadosOrigem.Criar(localizacaoUrl, nomeFisico, nomeOriginal, extensao);
    }
}
