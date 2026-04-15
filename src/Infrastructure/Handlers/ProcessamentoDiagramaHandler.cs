using Application.Contracts.Gateways;
using Application.Contracts.LLM;
using Application.Contracts.Messaging;
using Application.Contracts.Monitoramento;
using Application.ProcessamentoDiagrama.Dtos;
using Application.ProcessamentoDiagrama.UseCases;
using Domain.ProcessamentoDiagrama.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
using Shared.Constants;

namespace Infrastructure.Handlers;

public class ProcessamentoDiagramaHandler : BaseHandler
{
    public ProcessamentoDiagramaHandler(ILoggerFactory loggerFactory) : base(loggerFactory) { }

    public async Task IniciarProcessamentoAsync(ProcessarDiagramaDto processarDiagramaDto, IProcessamentoDiagramaGateway gateway, IDiagramaAnaliseService llmService, IProcessamentoDiagramaMessagePublisher messagePublisher, IMetricsService metrics, IAppLogger logger)
    {
        var processamentoExistente = await gateway.ObterPorAnaliseDiagramaIdAsync(processarDiagramaDto.AnaliseDiagramaId);

        if (processamentoExistente?.StatusProcessamento.Valor == StatusProcessamentoEnum.EmProcessamento)
        {
            logger.ComPropriedade(LogNomesPropriedades.AnaliseDiagramaId, processarDiagramaDto.AnaliseDiagramaId).LogInformation("Processamento já em andamento, ignorando mensagem duplicada para {AnaliseDiagramaId}", processarDiagramaDto.AnaliseDiagramaId);
            return;
        }

        if (processamentoExistente?.StatusProcessamento.Valor == StatusProcessamentoEnum.Concluido)
        {
            logger.ComPropriedade(LogNomesPropriedades.AnaliseDiagramaId, processarDiagramaDto.AnaliseDiagramaId).LogInformation("Processamento já concluído, ignorando mensagem duplicada para {AnaliseDiagramaId}", processarDiagramaDto.AnaliseDiagramaId);
            return;
        }

        if (processamentoExistente?.StatusProcessamento.Valor == StatusProcessamentoEnum.Rejeitado)
        {
            logger.ComPropriedade(LogNomesPropriedades.AnaliseDiagramaId, processarDiagramaDto.AnaliseDiagramaId).LogInformation("Processamento rejeitado (imagem inválida), ignorando para {AnaliseDiagramaId}", processarDiagramaDto.AnaliseDiagramaId);
            return;
        }

        processarDiagramaDto = TentarRecuperarLocalizacaoUrl(processarDiagramaDto, processamentoExistente, logger);

        if (string.IsNullOrWhiteSpace(processarDiagramaDto.LocalizacaoUrl))
        {
            logger.ComPropriedade(LogNomesPropriedades.AnaliseDiagramaId, processarDiagramaDto.AnaliseDiagramaId).LogWarning("Mensagem com LocalizacaoUrl vazia para {AnaliseDiagramaId}, ignorando mensagem com dados incompletos", processarDiagramaDto.AnaliseDiagramaId);
            return;
        }

        if (processamentoExistente == null)
        {
            try
            {
                var processamento = Domain.ProcessamentoDiagrama.Aggregates.ProcessamentoDiagrama.Criar(processarDiagramaDto.AnaliseDiagramaId);
                processamento.RegistrarDadosOrigem(processarDiagramaDto.LocalizacaoUrl, processarDiagramaDto.NomeFisico, processarDiagramaDto.NomeOriginal, processarDiagramaDto.Extensao);
                await gateway.SalvarAsync(processamento);
                logger.ComPropriedade(LogNomesPropriedades.AnaliseDiagramaId, processarDiagramaDto.AnaliseDiagramaId).LogDebug("Registro inicial de processamento criado para {AnaliseDiagramaId}", processarDiagramaDto.AnaliseDiagramaId);
            }
            catch (DbUpdateException ex) when (ex.InnerException is PostgresException { SqlState: "23505" })
            {
                logger.ComPropriedade(LogNomesPropriedades.AnaliseDiagramaId, processarDiagramaDto.AnaliseDiagramaId).LogWarning("Mensagem duplicada detectada (constraint violation), ignorando para {AnaliseDiagramaId}", processarDiagramaDto.AnaliseDiagramaId);
                return;
            }
        }

        await ProcessarDiagramaAsync(processarDiagramaDto, gateway, llmService, messagePublisher, metrics);
    }

    private static ProcessarDiagramaDto TentarRecuperarLocalizacaoUrl(ProcessarDiagramaDto dto, Domain.ProcessamentoDiagrama.Aggregates.ProcessamentoDiagrama? processamentoExistente, IAppLogger logger)
    {
        if (!string.IsNullOrWhiteSpace(dto.LocalizacaoUrl))
            return dto;

        if (processamentoExistente?.DadosOrigem == null)
            return dto;

        var urlRecuperada = processamentoExistente.DadosOrigem.LocalizacaoUrl.Valor;

        if (string.IsNullOrWhiteSpace(urlRecuperada))
            return dto;

        logger.ComPropriedade(LogNomesPropriedades.AnaliseDiagramaId, dto.AnaliseDiagramaId).ComPropriedade(LogNomesPropriedades.LocalizacaoUrl, urlRecuperada).LogWarning("LocalizacaoUrl recuperada do banco de dados para retry de {AnaliseDiagramaId}. {LocalizacaoUrl}: {LocalizacaoUrl}", dto.AnaliseDiagramaId, urlRecuperada);

        return dto with
        {
            LocalizacaoUrl = urlRecuperada,
            NomeFisico = string.IsNullOrWhiteSpace(dto.NomeFisico) ? processamentoExistente.DadosOrigem.NomeFisico.Valor : dto.NomeFisico,
            NomeOriginal = string.IsNullOrWhiteSpace(dto.NomeOriginal) ? processamentoExistente.DadosOrigem.NomeOriginal.Valor : dto.NomeOriginal,
            Extensao = string.IsNullOrWhiteSpace(dto.Extensao) ? processamentoExistente.DadosOrigem.Extensao.Valor : dto.Extensao
        };
    }

    public async Task ProcessarDiagramaAsync(ProcessarDiagramaDto processarDiagramaDto, IProcessamentoDiagramaGateway gateway, IDiagramaAnaliseService llmService, IProcessamentoDiagramaMessagePublisher messagePublisher, IMetricsService metrics)
    {
        var useCase = new ProcessarDiagramaUseCase();
        var logger = CriarLoggerPara<ProcessarDiagramaUseCase>();

        await useCase.ExecutarAsync(processarDiagramaDto, gateway, llmService, messagePublisher, metrics, logger);
    }
}
