using Application.Contracts.Messaging;
using Application.Contracts.Messaging.Dtos;
using Application.Contracts.Monitoramento;
using MassTransit;

namespace Infrastructure.Messaging;

/// <summary>
/// Implementação do publisher de mensagens de processamento usando MassTransit com Amazon SQS.
/// </summary>
public class ProcessamentoDiagramaMessagePublisher : IProcessamentoDiagramaMessagePublisher
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ICorrelationIdAccessor _correlationIdAccessor;

    public ProcessamentoDiagramaMessagePublisher(IPublishEndpoint publishEndpoint, ICorrelationIdAccessor correlationIdAccessor)
    {
        _publishEndpoint = publishEndpoint;
        _correlationIdAccessor = correlationIdAccessor;
    }

    public async Task PublicarProcessamentoIniciadoAsync(Domain.ProcessamentoDiagrama.Aggregates.ProcessamentoDiagrama processamento, string nomeOriginal, string extensao)
    {
        var mensagem = new ProcessamentoDiagramaIniciadoDto
        {
            CorrelationId = _correlationIdAccessor.GetCorrelationId(),
            AnaliseDiagramaId = processamento.AnaliseDiagramaId,
            NomeOriginal = nomeOriginal,
            Extensao = extensao,
            DataInicio = processamento.HistoricoTemporal.DataInicioProcessamento ?? DateTimeOffset.UtcNow
        };

        await _publishEndpoint.Publish(mensagem);
    }

    public async Task PublicarDiagramaAnalisadoAsync(Domain.ProcessamentoDiagrama.Aggregates.ProcessamentoDiagrama processamento)
    {
        var mensagem = new ProcessamentoDiagramaAnalisadoDto
        {
            CorrelationId = _correlationIdAccessor.GetCorrelationId(),
            AnaliseDiagramaId = processamento.AnaliseDiagramaId,
            DescricaoAnalise = processamento.AnaliseResultado?.DescricaoAnalise.Valor ?? string.Empty,
            ComponentesIdentificados = processamento.AnaliseResultado?.ComponentesIdentificados.Select(item => item.Valor).ToList() ?? new List<string>(),
            RiscosArquiteturais = processamento.AnaliseResultado?.RiscosArquiteturais.Select(item => item.Valor).ToList() ?? new List<string>(),
            RecomendacoesBasicas = processamento.AnaliseResultado?.RecomendacoesBasicas.Select(item => item.Valor).ToList() ?? new List<string>(),
            DataConclusao = processamento.HistoricoTemporal.DataConclusaoProcessamento ?? DateTimeOffset.UtcNow
        };

        await _publishEndpoint.Publish(mensagem);
    }

    public async Task PublicarProcessamentoErroAsync(Domain.ProcessamentoDiagrama.Aggregates.ProcessamentoDiagrama processamento, string motivo)
    {
        var mensagem = new ProcessamentoDiagramaErroDto
        {
            CorrelationId = _correlationIdAccessor.GetCorrelationId(),
            AnaliseDiagramaId = processamento.AnaliseDiagramaId,
            Motivo = motivo,
            TentativasRealizadas = processamento.TentativasProcessamento.Valor,
            DataErro = processamento.HistoricoTemporal.DataConclusaoProcessamento ?? DateTimeOffset.UtcNow
        };

        await _publishEndpoint.Publish(mensagem);
    }
}
