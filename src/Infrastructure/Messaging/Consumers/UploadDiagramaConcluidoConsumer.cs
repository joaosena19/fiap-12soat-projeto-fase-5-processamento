using Application.Contracts.Gateways;
using Application.Contracts.LLM;
using Application.Contracts.Messaging;
using Application.Contracts.Messaging.Dtos;
using Application.Contracts.Monitoramento;
using Application.Extensions;
using Application.ProcessamentoDiagrama.Dtos;
using Infrastructure.Handlers;
using Infrastructure.Monitoramento;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Shared.Constants;

namespace Infrastructure.Messaging;

/// <summary>
/// Consumer MassTransit que consome mensagens de upload concluído e inicia o processamento.
/// </summary>
public class UploadDiagramaConcluidoConsumer : IConsumer<UploadDiagramaConcluidoDto>
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILoggerFactory _loggerFactory;

    public UploadDiagramaConcluidoConsumer(IServiceProvider serviceProvider, ILoggerFactory loggerFactory)
    {
        _serviceProvider = serviceProvider;
        _loggerFactory = loggerFactory;
    }

    public async Task Consume(ConsumeContext<UploadDiagramaConcluidoDto> context)
    {
        var mensagem = context.Message;
        var logger = new LoggerAdapter<UploadDiagramaConcluidoConsumer>(_loggerFactory.CreateLogger<UploadDiagramaConcluidoConsumer>());

        try
        {
            var handler = new ProcessamentoDiagramaHandler(_loggerFactory);
            var gateway = _serviceProvider.GetRequiredService<IProcessamentoDiagramaGateway>();
            var llmService = _serviceProvider.GetRequiredService<IDiagramaAnaliseService>();
            var messagePublisher = _serviceProvider.GetRequiredService<IProcessamentoDiagramaMessagePublisher>();
            var metrics = _serviceProvider.GetRequiredService<IMetricsService>();
            var messageId = context.MessageId?.ToString() ?? "desconhecido";

            logger.ComConsumoMensagem(this).ComPropriedade(LogNomesPropriedades.AnaliseDiagramaId, mensagem.AnaliseDiagramaId).ComPropriedade(LogNomesPropriedades.MessageId, messageId).LogInformation($"Recebida mensagem de upload concluído para processamento. {{{LogNomesPropriedades.MessageId}}}", messageId);

            var processamentoExistente = await gateway.ObterPorAnaliseDiagramaIdAsync(mensagem.AnaliseDiagramaId);

            if (processamentoExistente == null)
            {
                var processamento = Domain.ProcessamentoDiagrama.Aggregates.ProcessamentoDiagrama.Criar(mensagem.AnaliseDiagramaId);

                await gateway.SalvarAsync(processamento);
                logger.ComConsumoMensagem(this).ComPropriedade(LogNomesPropriedades.AnaliseDiagramaId, mensagem.AnaliseDiagramaId).LogDebug("Registro inicial de processamento criado.");
            }

            var processarDiagramaDto = new ProcessarDiagramaDto
            {
                AnaliseDiagramaId = mensagem.AnaliseDiagramaId,
                NomeOriginal = mensagem.NomeOriginal,
                Extensao = mensagem.Extensao,
                NomeFisico = mensagem.NomeFisico,
                LocalizacaoUrl = mensagem.LocalizacaoUrl
            };

            await handler.ProcessarDiagramaAsync(processarDiagramaDto, gateway, llmService, messagePublisher, metrics);
        }
        catch (Exception ex)
        {
            logger.ComConsumoMensagem(this).ComPropriedade(LogNomesPropriedades.AnaliseDiagramaId, mensagem.AnaliseDiagramaId).LogError(ex, "Erro ao consumir mensagem de upload concluído");
            throw;
        }
    }
}
