using Application.Contracts.Gateways;
using Application.Contracts.LLM;
using Application.Contracts.Messaging;
using Application.Contracts.Monitoramento;
using Application.ProcessamentoDiagrama.Dtos;
using Application.Extensions;
using Shared.Constants;
using Shared.Enums;
using Shared.Exceptions;
using Domain.ProcessamentoDiagrama.Aggregates;

namespace Application.ProcessamentoDiagrama.UseCases;

public class ProcessarDiagramaUseCase
{
    public async Task ExecutarAsync(ProcessarDiagramaDto processarDiagramaDto, IProcessamentoDiagramaGateway gateway, IDiagramaAnaliseService llmService, IProcessamentoDiagramaMessagePublisher messagePublisher, IMetricsService metrics, IAppLogger logger)
    {
        var analiseDiagramaId = processarDiagramaDto.AnaliseDiagramaId;

        try
        {
            logger.ComUseCase(this).ComPropriedade(LogNomesPropriedades.AnaliseDiagramaId, analiseDiagramaId).LogDebug($"Iniciando processamento do diagrama {{{LogNomesPropriedades.AnaliseDiagramaId}}}", analiseDiagramaId);

            var processamentoDiagrama = await ObterProcessamentoValidadoAsync(analiseDiagramaId, gateway);

            await SinalizarInicioProcessamentoAsync(processamentoDiagrama, processarDiagramaDto, gateway, messagePublisher, metrics);

            var inicioProcessamento = DateTimeOffset.UtcNow;
            var resultado = await llmService.AnalisarDiagramaAsync(analiseDiagramaId, processarDiagramaDto.NomeFisico, processarDiagramaDto.LocalizacaoUrl, processarDiagramaDto.Extensao);

            if (resultado.Sucesso)
                await TratarSucessoAsync(processamentoDiagrama, resultado, inicioProcessamento, gateway, messagePublisher, metrics, logger);
            else
                await TratarFalhaAsync(processamentoDiagrama, resultado, gateway, messagePublisher, metrics, logger);
        }
        catch (DomainException ex)
        {
            logger.ComUseCase(this).ComPropriedade(LogNomesPropriedades.AnaliseDiagramaId, analiseDiagramaId).ComDomainErrorType(ex).LogError(ex, ex.LogTemplate, ex.LogArgs);
            throw;
        }
        catch (Exception ex)
        {
            logger.ComUseCase(this).ComPropriedade(LogNomesPropriedades.AnaliseDiagramaId, analiseDiagramaId).LogError(ex, $"Erro interno ao processar diagrama {{{LogNomesPropriedades.AnaliseDiagramaId}}}.", analiseDiagramaId);
            throw;
        }
    }

    private async Task<Domain.ProcessamentoDiagrama.Aggregates.ProcessamentoDiagrama> ObterProcessamentoValidadoAsync(Guid analiseDiagramaId, IProcessamentoDiagramaGateway gateway)
    {
        var processamentoDiagrama = await gateway.ObterPorAnaliseDiagramaIdAsync(analiseDiagramaId);

        if (processamentoDiagrama == null)
            throw new DomainException("Processamento não encontrado", ErrorType.ResourceNotFound);

        return processamentoDiagrama;
    }

    private async Task SinalizarInicioProcessamentoAsync(Domain.ProcessamentoDiagrama.Aggregates.ProcessamentoDiagrama processamentoDiagrama, ProcessarDiagramaDto dto, IProcessamentoDiagramaGateway gateway, IProcessamentoDiagramaMessagePublisher messagePublisher, IMetricsService metrics)
    {
        processamentoDiagrama.IniciarProcessamento();
        await gateway.SalvarAsync(processamentoDiagrama);
        await messagePublisher.PublicarProcessamentoIniciadoAsync(processamentoDiagrama, dto.NomeOriginal, dto.Extensao);
        metrics.RegistrarProcessamentoIniciado(processamentoDiagrama.AnaliseDiagramaId);
    }

    private async Task TratarSucessoAsync(Domain.ProcessamentoDiagrama.Aggregates.ProcessamentoDiagrama processamentoDiagrama, ResultadoAnaliseDto resultado, DateTimeOffset inicioProcessamento, IProcessamentoDiagramaGateway gateway, IProcessamentoDiagramaMessagePublisher messagePublisher, IMetricsService metrics, IAppLogger logger)
    {
        processamentoDiagrama.ConcluirProcessamento(resultado.DescricaoAnalise!, resultado.ComponentesIdentificados, resultado.RiscosArquiteturais, resultado.RecomendacoesBasicas, resultado.TentativasRealizadas);

        await gateway.SalvarAsync(processamentoDiagrama);
        await messagePublisher.PublicarDiagramaAnalisadoAsync(processamentoDiagrama);

        var duracaoMs = (long)(DateTimeOffset.UtcNow - inicioProcessamento).TotalMilliseconds;
        metrics.RegistrarProcessamentoConcluido(processamentoDiagrama.AnaliseDiagramaId, duracaoMs);

        logger.ComUseCase(this).ComPropriedade(LogNomesPropriedades.AnaliseDiagramaId, processamentoDiagrama.AnaliseDiagramaId).ComPropriedade(LogNomesPropriedades.DuracaoMs, duracaoMs).LogDebug($"Processamento concluído com sucesso para {{{LogNomesPropriedades.AnaliseDiagramaId}}} em {{{LogNomesPropriedades.DuracaoMs}}}ms", processamentoDiagrama.AnaliseDiagramaId, duracaoMs);
    }

    private async Task TratarFalhaAsync(Domain.ProcessamentoDiagrama.Aggregates.ProcessamentoDiagrama processamentoDiagrama, ResultadoAnaliseDto resultado, IProcessamentoDiagramaGateway gateway, IProcessamentoDiagramaMessagePublisher messagePublisher, IMetricsService metrics, IAppLogger logger)
    {
        var motivo = resultado.MotivoErro ?? "Erro desconhecido na análise do diagrama";

        processamentoDiagrama.RegistrarFalha(resultado.TentativasRealizadas);
        await gateway.SalvarAsync(processamentoDiagrama);
        await messagePublisher.PublicarProcessamentoErroAsync(processamentoDiagrama, motivo);

        metrics.RegistrarProcessamentoFalha(processamentoDiagrama.AnaliseDiagramaId, motivo, resultado.TentativasRealizadas);

        logger.ComUseCase(this).ComPropriedade(LogNomesPropriedades.AnaliseDiagramaId, processamentoDiagrama.AnaliseDiagramaId).ComPropriedade(LogNomesPropriedades.Tentativas, resultado.TentativasRealizadas).LogError($"Processamento finalizado com falha para {{{LogNomesPropriedades.AnaliseDiagramaId}}}. {LogNomesPropriedades.Motivo}: {{{LogNomesPropriedades.Motivo}}}. {LogNomesPropriedades.Tentativas}: {{{LogNomesPropriedades.Tentativas}}}", processamentoDiagrama.AnaliseDiagramaId, motivo, resultado.TentativasRealizadas);
    }
}