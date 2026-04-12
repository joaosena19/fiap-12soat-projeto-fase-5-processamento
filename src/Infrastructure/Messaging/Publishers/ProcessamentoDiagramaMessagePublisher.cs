using Application.Contracts.Messaging;
using Application.Contracts.Messaging.Dtos;
using Application.Contracts.Monitoramento;
using Application.Extensions;
using Infrastructure.Monitoramento;
using MassTransit;
using Microsoft.Extensions.Logging;
using Shared.Constants;

namespace Infrastructure.Messaging.Publishers;

/// <summary>
/// Implementação do publisher de mensagens de processamento usando MassTransit com Amazon SQS.
/// </summary>
public class ProcessamentoDiagramaMessagePublisher : IProcessamentoDiagramaMessagePublisher
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ICorrelationIdAccessor _correlationIdAccessor;
    private readonly IAppLogger _logger;

    public ProcessamentoDiagramaMessagePublisher(IPublishEndpoint publishEndpoint, ICorrelationIdAccessor correlationIdAccessor, ILoggerFactory loggerFactory)
    {
        _publishEndpoint = publishEndpoint;
        _correlationIdAccessor = correlationIdAccessor;
        _logger = loggerFactory.CriarAppLogger<ProcessamentoDiagramaMessagePublisher>();
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

        _logger.ComEnvioMensagem(this).ComPropriedade(LogNomesPropriedades.AnaliseDiagramaId, processamento.AnaliseDiagramaId).ComPropriedade(LogNomesPropriedades.Extensao, extensao).LogInformation($"Publicando evento de processamento iniciado para {{{LogNomesPropriedades.AnaliseDiagramaId}}}", processamento.AnaliseDiagramaId);

        try
        {
            await _publishEndpoint.Publish(mensagem);
        }
        catch (Exception ex)
        {
            _logger.ComEnvioMensagem(this).ComPropriedade(LogNomesPropriedades.AnaliseDiagramaId, processamento.AnaliseDiagramaId).LogError(ex, $"Falha ao publicar mensagem para {{{LogNomesPropriedades.AnaliseDiagramaId}}}", processamento.AnaliseDiagramaId);
            throw;
        }
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

        _logger.ComEnvioMensagem(this).ComPropriedade(LogNomesPropriedades.AnaliseDiagramaId, processamento.AnaliseDiagramaId).LogInformation($"Publicando evento de processamento analisado para {{{LogNomesPropriedades.AnaliseDiagramaId}}}", processamento.AnaliseDiagramaId);

        try
        {
            await _publishEndpoint.Publish(mensagem);
        }
        catch (Exception ex)
        {
            _logger.ComEnvioMensagem(this).ComPropriedade(LogNomesPropriedades.AnaliseDiagramaId, processamento.AnaliseDiagramaId).LogError(ex, $"Falha ao publicar mensagem para {{{LogNomesPropriedades.AnaliseDiagramaId}}}", processamento.AnaliseDiagramaId);
            throw;
        }
    }

    public async Task PublicarProcessamentoErroAsync(Domain.ProcessamentoDiagrama.Aggregates.ProcessamentoDiagrama processamento, string motivo, string? origemErro = null)
    {
        var mensagem = new ProcessamentoDiagramaErroDto
        {
            CorrelationId = _correlationIdAccessor.GetCorrelationId(),
            AnaliseDiagramaId = processamento.AnaliseDiagramaId,
            Motivo = motivo,
            OrigemErro = origemErro ?? OrigemErroConstantes.Processamento,
            TentativasRealizadas = processamento.TentativasProcessamento.Valor,
            DataErro = processamento.HistoricoTemporal.DataConclusaoProcessamento ?? DateTimeOffset.UtcNow
        };

        _logger.ComEnvioMensagem(this).ComPropriedade(LogNomesPropriedades.AnaliseDiagramaId, processamento.AnaliseDiagramaId).ComPropriedade(LogNomesPropriedades.Motivo, motivo).LogInformation($"Publicando evento de processamento com erro para {{{LogNomesPropriedades.AnaliseDiagramaId}}}", processamento.AnaliseDiagramaId);

        try
        {
            await _publishEndpoint.Publish(mensagem);
        }
        catch (Exception ex)
        {
            _logger.ComEnvioMensagem(this).ComPropriedade(LogNomesPropriedades.AnaliseDiagramaId, processamento.AnaliseDiagramaId).LogError(ex, $"Falha ao publicar mensagem para {{{LogNomesPropriedades.AnaliseDiagramaId}}}", processamento.AnaliseDiagramaId);
            throw;
        }
    }
}
