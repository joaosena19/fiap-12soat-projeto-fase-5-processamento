using Application.Contracts.Gateways;
using Application.Contracts.LLM;
using Application.Contracts.Messaging;
using Application.Contracts.Monitoramento;
using Application.ProcessamentoDiagrama.Dtos;
using Application.ProcessamentoDiagrama.UseCases;
using Domain.ProcessamentoDiagrama.Enums;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Handlers;

public class ProcessamentoDiagramaHandler : BaseHandler
{
    public ProcessamentoDiagramaHandler(ILoggerFactory loggerFactory) : base(loggerFactory) { }

    public async Task IniciarProcessamentoAsync(ProcessarDiagramaDto processarDiagramaDto, IProcessamentoDiagramaGateway gateway, IDiagramaAnaliseService llmService, IProcessamentoDiagramaMessagePublisher messagePublisher, IMetricsService metrics, IAppLogger logger)
    {
        var processamentoExistente = await gateway.ObterPorAnaliseDiagramaIdAsync(processarDiagramaDto.AnaliseDiagramaId);

        if (processamentoExistente?.StatusProcessamento.Valor == StatusProcessamentoEnum.EmProcessamento)
        {
            logger.LogWarning("Processamento já está em andamento para o AnaliseDiagramaId, ignorando mensagem duplicada");
            return;
        }

        if (processamentoExistente?.StatusProcessamento.Valor == StatusProcessamentoEnum.Concluido)
        {
            logger.LogWarning("Processamento já foi concluído para o AnaliseDiagramaId, ignorando mensagem duplicada");
            return;
        }

        if (processamentoExistente == null)
        {
            var processamento = Domain.ProcessamentoDiagrama.Aggregates.ProcessamentoDiagrama.Criar(processarDiagramaDto.AnaliseDiagramaId);
            await gateway.SalvarAsync(processamento);
            logger.LogDebug("Registro inicial de processamento criado.");
        }

        await ProcessarDiagramaAsync(processarDiagramaDto, gateway, llmService, messagePublisher, metrics);
    }

    public async Task ProcessarDiagramaAsync(ProcessarDiagramaDto processarDiagramaDto, IProcessamentoDiagramaGateway gateway, IDiagramaAnaliseService llmService, IProcessamentoDiagramaMessagePublisher messagePublisher, IMetricsService metrics)
    {
        var useCase = new ProcessarDiagramaUseCase();
        var logger = CriarLoggerPara<ProcessarDiagramaUseCase>();

        await useCase.ExecutarAsync(processarDiagramaDto, gateway, llmService, messagePublisher, metrics, logger);
    }
}
