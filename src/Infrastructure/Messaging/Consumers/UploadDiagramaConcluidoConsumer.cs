using Application.Contracts.LLM;
using Application.Contracts.Messaging;
using Application.Contracts.Messaging.Dtos;
using Application.Extensions;
using Application.ProcessamentoDiagrama.Dtos;
using Infrastructure.Database;
using Infrastructure.Handlers;
using Infrastructure.Monitoramento;
using Infrastructure.Repositories;
using MassTransit;
using Microsoft.Extensions.Logging;
using Shared.Constants;

namespace Infrastructure.Messaging.Consumers;

/// <summary>
/// Consumer MassTransit que consome mensagens de upload concluído e inicia o processamento.
/// </summary>
public class UploadDiagramaConcluidoConsumer : IConsumer<UploadDiagramaConcluidoDto>
{
    private readonly AppDbContext _context;
    private readonly IDiagramaAnaliseService _llmService;
    private readonly IProcessamentoDiagramaMessagePublisher _messagePublisher;
    private readonly ILoggerFactory _loggerFactory;

    public UploadDiagramaConcluidoConsumer(AppDbContext context, IDiagramaAnaliseService llmService, IProcessamentoDiagramaMessagePublisher messagePublisher, ILoggerFactory loggerFactory)
    {
        _context = context;
        _llmService = llmService;
        _messagePublisher = messagePublisher;
        _loggerFactory = loggerFactory;
    }

    public async Task Consume(ConsumeContext<UploadDiagramaConcluidoDto> context)
    {
        var mensagem = context.Message;
        var logger = _loggerFactory.CriarAppLogger<UploadDiagramaConcluidoConsumer>();

        try
        {
            var handler = new ProcessamentoDiagramaHandler(_loggerFactory);
            var gateway = new ProcessamentoDiagramaRepository(_context);
            var metrics = new NewRelicMetricsService();
            var messageId = context.MessageId?.ToString() ?? LogNomesValores.Desconhecido;

            logger.ComConsumoMensagem(this).ComPropriedade(LogNomesPropriedades.AnaliseDiagramaId, mensagem.AnaliseDiagramaId).ComPropriedade(LogNomesPropriedades.MessageId, messageId).LogInformation($"Recebida mensagem de upload concluído para processamento. {{{LogNomesPropriedades.MessageId}}}", messageId);

            var processarDiagramaDto = new ProcessarDiagramaDto
            {
                AnaliseDiagramaId = mensagem.AnaliseDiagramaId,
                NomeOriginal = mensagem.NomeOriginal,
                Extensao = mensagem.Extensao,
                NomeFisico = mensagem.NomeFisico,
                LocalizacaoUrl = mensagem.LocalizacaoUrl
            };

            await handler.IniciarProcessamentoAsync(processarDiagramaDto, gateway, _llmService, _messagePublisher, metrics, logger);
        }
        catch (Exception ex)
        {
            logger.ComConsumoMensagem(this).ComPropriedade(LogNomesPropriedades.AnaliseDiagramaId, mensagem.AnaliseDiagramaId).LogError(ex, "Erro ao consumir mensagem de upload concluído");
            throw;
        }
    }
}
