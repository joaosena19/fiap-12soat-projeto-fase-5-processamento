using Shared.Attributes;
using Shared.Enums;
using Shared.Exceptions;

namespace Domain.ProcessamentoDiagrama.ValueObjects;

[ValueObject]
public record HistoricoTemporal
{
    public DateTimeOffset DataCriacao { get; private init; }
    public DateTimeOffset? DataInicioProcessamento { get; private init; }
    public DateTimeOffset? DataConclusaoProcessamento { get; private init; }

    private HistoricoTemporal() { }

    private HistoricoTemporal(DateTimeOffset dataCriacao, DateTimeOffset? dataInicioProcessamento = null, DateTimeOffset? dataConclusaoProcessamento = null)
    {
        if (dataCriacao == default)
            throw new DomainException("A data de criação é obrigatória.", ErrorType.InvalidInput);

        if (dataInicioProcessamento.HasValue && dataInicioProcessamento.Value < dataCriacao)
            throw new DomainException("A data de início do processamento não pode ser anterior à data de criação.", ErrorType.DomainRuleBroken);

        if (dataConclusaoProcessamento.HasValue && dataInicioProcessamento.HasValue && dataConclusaoProcessamento.Value < dataInicioProcessamento.Value)
            throw new DomainException("A data de conclusão do processamento não pode ser anterior à data de início.", ErrorType.DomainRuleBroken);

        DataCriacao = dataCriacao;
        DataInicioProcessamento = dataInicioProcessamento;
        DataConclusaoProcessamento = dataConclusaoProcessamento;
    }

    public static HistoricoTemporal Criar(DateTimeOffset? dataCriacao = null) => new(dataCriacao ?? DateTimeOffset.UtcNow);

    public static HistoricoTemporal Reidratar(DateTimeOffset dataCriacao, DateTimeOffset? dataInicioProcessamento, DateTimeOffset? dataConclusaoProcessamento) => new(dataCriacao, dataInicioProcessamento, dataConclusaoProcessamento);

    public HistoricoTemporal MarcarInicioProcessamento(DateTimeOffset? data = null) => new(DataCriacao, data ?? DateTimeOffset.UtcNow, null);

    public HistoricoTemporal MarcarConclusaoProcessamento(DateTimeOffset? data = null) => new(DataCriacao, DataInicioProcessamento, data ?? DateTimeOffset.UtcNow);
}