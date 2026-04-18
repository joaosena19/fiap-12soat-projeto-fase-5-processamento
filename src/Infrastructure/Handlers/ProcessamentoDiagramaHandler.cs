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
    private static readonly HashSet<StatusProcessamentoEnum> StatusTerminaisOuEmAndamento =
    [
        StatusProcessamentoEnum.EmProcessamento,
        StatusProcessamentoEnum.Concluido,
        StatusProcessamentoEnum.Rejeitado
    ];

    public ProcessamentoDiagramaHandler(ILoggerFactory loggerFactory) : base(loggerFactory) { }

    public async Task IniciarProcessamentoAsync(ProcessarDiagramaDto processarDiagramaDto, IProcessamentoDiagramaGateway gateway, IDiagramaAnaliseService llmService, IProcessamentoDiagramaMessagePublisher messagePublisher, IMetricsService metrics, IAppLogger logger)
    {
        var processamentoExistente = await gateway.ObterPorAnaliseDiagramaIdAsync(processarDiagramaDto.AnaliseDiagramaId);

        if (DeveIgnorar(processamentoExistente, processarDiagramaDto.AnaliseDiagramaId, logger))
            return;

        processarDiagramaDto = TentarRecuperarLocalizacaoUrl(processarDiagramaDto, processamentoExistente, logger);

        if (!await ValidarLocalizacaoUrlAsync(processarDiagramaDto, processamentoExistente, messagePublisher, logger))
            return;

        if (processamentoExistente == null)
            if (!await TentarCriarRegistroInicialAsync(processarDiagramaDto, gateway, logger))
                return;

        await ProcessarDiagramaAsync(processarDiagramaDto, gateway, llmService, messagePublisher, metrics);
    }

    private static bool DeveIgnorar(Domain.ProcessamentoDiagrama.Aggregates.ProcessamentoDiagrama? processamentoExistente, Guid analiseDiagramaId, IAppLogger logger)
    {
        if (processamentoExistente == null)
            return false;

        var status = processamentoExistente.StatusProcessamento.Valor;
        if (!StatusTerminaisOuEmAndamento.Contains(status))
            return false;

        var motivo = status switch
        {
            StatusProcessamentoEnum.EmProcessamento => "Processamento já em andamento, ignorando mensagem duplicada",
            StatusProcessamentoEnum.Concluido => "Processamento já concluído, ignorando mensagem duplicada",
            StatusProcessamentoEnum.Rejeitado => "Processamento rejeitado (imagem inválida), ignorando",
            _ => "Status terminal detectado, ignorando"
        };

        logger.ComPropriedade(LogNomesPropriedades.AnaliseDiagramaId, analiseDiagramaId).LogInformation(motivo + " para {AnaliseDiagramaId}", analiseDiagramaId);
        return true;
    }

    private static async Task<bool> ValidarLocalizacaoUrlAsync(ProcessarDiagramaDto processarDiagramaDto, Domain.ProcessamentoDiagrama.Aggregates.ProcessamentoDiagrama? processamentoExistente, IProcessamentoDiagramaMessagePublisher messagePublisher, IAppLogger logger)
    {
        if (!string.IsNullOrWhiteSpace(processarDiagramaDto.LocalizacaoUrl))
            return true;

        if (processamentoExistente != null)
        {
            logger.ComPropriedade(LogNomesPropriedades.AnaliseDiagramaId, processarDiagramaDto.AnaliseDiagramaId).LogWarning("LocalizacaoUrl vazia e irrecuperável para {AnaliseDiagramaId}. Publicando erro por dados de origem incompletos.", processarDiagramaDto.AnaliseDiagramaId);
            await messagePublisher.PublicarProcessamentoErroAsync(processamentoExistente, "LocalizacaoUrl não disponível — dados de origem não encontrados para recuperação", podeRetentar: false);
        }
        else
            logger.ComPropriedade(LogNomesPropriedades.AnaliseDiagramaId, processarDiagramaDto.AnaliseDiagramaId).LogWarning("Mensagem com LocalizacaoUrl vazia para {AnaliseDiagramaId}, ignorando mensagem com dados incompletos", processarDiagramaDto.AnaliseDiagramaId);

        return false;
    }

    private static async Task<bool> TentarCriarRegistroInicialAsync(ProcessarDiagramaDto processarDiagramaDto, IProcessamentoDiagramaGateway gateway, IAppLogger logger)
    {
        try
        {
            var processamento = Domain.ProcessamentoDiagrama.Aggregates.ProcessamentoDiagrama.Criar(processarDiagramaDto.AnaliseDiagramaId);
            processamento.RegistrarDadosOrigem(processarDiagramaDto.LocalizacaoUrl, processarDiagramaDto.NomeFisico, processarDiagramaDto.NomeOriginal, processarDiagramaDto.Extensao);
            await gateway.SalvarAsync(processamento);
            logger.ComPropriedade(LogNomesPropriedades.AnaliseDiagramaId, processarDiagramaDto.AnaliseDiagramaId).LogDebug("Registro inicial de processamento criado para {AnaliseDiagramaId}", processarDiagramaDto.AnaliseDiagramaId);
            return true;
        }
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException { SqlState: "23505" })
        {
            logger.ComPropriedade(LogNomesPropriedades.AnaliseDiagramaId, processarDiagramaDto.AnaliseDiagramaId).LogWarning("Mensagem duplicada detectada (constraint violation), ignorando para {AnaliseDiagramaId}", processarDiagramaDto.AnaliseDiagramaId);
            return false;
        }
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
