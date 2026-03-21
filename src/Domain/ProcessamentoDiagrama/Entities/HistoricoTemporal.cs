using Shared.Attributes;
using Shared.Enums;
using Shared.Exceptions;

namespace Domain.ProcessamentoDiagrama.Entities;

[AggregateMember]
public class HistoricoTemporal
{
    private DateTimeOffset _dataCriacao;
    private DateTimeOffset? _dataInicioProcessamento;
    private DateTimeOffset? _dataConclusaoProcessamento;

    private HistoricoTemporal() { }

    private HistoricoTemporal(DateTimeOffset dataCriacao, DateTimeOffset? dataInicioProcessamento = null, DateTimeOffset? dataConclusaoProcessamento = null)
    {
        if (dataCriacao == default)
            throw new DomainException("A data de criação é obrigatória.", ErrorType.InvalidInput);

        if (dataInicioProcessamento.HasValue && dataInicioProcessamento.Value < dataCriacao)
            throw new DomainException("A data de início do processamento não pode ser anterior à data de criação.", ErrorType.DomainRuleBroken);

        if (dataConclusaoProcessamento.HasValue && dataInicioProcessamento.HasValue && dataConclusaoProcessamento.Value < dataInicioProcessamento.Value)
            throw new DomainException("A data de conclusão do processamento não pode ser anterior à data de início.", ErrorType.DomainRuleBroken);

        _dataCriacao = dataCriacao;
        _dataInicioProcessamento = dataInicioProcessamento;
        _dataConclusaoProcessamento = dataConclusaoProcessamento;
    }

    public DateTimeOffset DataCriacao => _dataCriacao;
    public DateTimeOffset? DataInicioProcessamento => _dataInicioProcessamento;
    public DateTimeOffset? DataConclusaoProcessamento => _dataConclusaoProcessamento;

    public static HistoricoTemporal Criar(DateTimeOffset? dataCriacao = null)
        => new(dataCriacao ?? DateTimeOffset.UtcNow);

    public static HistoricoTemporal Reidratar(DateTimeOffset dataCriacao, DateTimeOffset? dataInicioProcessamento, DateTimeOffset? dataConclusaoProcessamento)
    {
        return new HistoricoTemporal
        {
            _dataCriacao = dataCriacao,
            _dataInicioProcessamento = dataInicioProcessamento,
            _dataConclusaoProcessamento = dataConclusaoProcessamento
        };
    }

    public HistoricoTemporal MarcarInicioProcessamento(DateTimeOffset? data = null)
        => new(_dataCriacao, data ?? DateTimeOffset.UtcNow, _dataConclusaoProcessamento);

    public HistoricoTemporal MarcarConclusaoProcessamento(DateTimeOffset? data = null)
        => new(_dataCriacao, _dataInicioProcessamento, data ?? DateTimeOffset.UtcNow);
}