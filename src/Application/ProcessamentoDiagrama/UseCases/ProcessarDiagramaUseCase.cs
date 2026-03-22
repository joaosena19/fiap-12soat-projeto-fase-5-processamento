using Application.Contracts.Gateways;
using Application.Contracts.LLM;
using Application.Contracts.Messaging;
using Application.Contracts.Monitoramento;
using Application.ProcessamentoDiagrama.Dtos;
using Application.Extensions;
using Shared.Constants;
using Shared.Enums;
using Shared.Exceptions;

namespace Application.ProcessamentoDiagrama.UseCases;

public class ProcessarDiagramaUseCase
{
    public async Task ExecutarAsync(ProcessarDiagramaDto processarDiagramaDto, IProcessamentoDiagramaGateway gateway, IDiagramaAnaliseService llmService, IProcessamentoDiagramaMessagePublisher messagePublisher, IMetricsService metrics, IAppLogger logger)
    {
        var analiseDiagramaId = processarDiagramaDto.AnaliseDiagramaId;

        try
        {
            logger.ComUseCase(this).ComPropriedade(LogNomesPropriedades.AnaliseDiagramaId, analiseDiagramaId).LogDebug($"Iniciando processamento do diagrama {{{LogNomesPropriedades.AnaliseDiagramaId}}}", analiseDiagramaId);

            var processamento = await gateway.ObterPorAnaliseDiagramaIdAsync(analiseDiagramaId);
            if (processamento == null)
                throw new DomainException("Processamento não encontrado", ErrorType.ResourceNotFound);

            processamento.IniciarProcessamento();
            await gateway.SalvarAsync(processamento);
            await messagePublisher.PublicarProcessamentoIniciadoAsync(processamento, processarDiagramaDto.NomeOriginal, processarDiagramaDto.Extensao);

            metrics.RegistrarProcessamentoIniciado(analiseDiagramaId);

            var inicioProcessamento = DateTimeOffset.UtcNow;
            var resultado = await llmService.AnalisarDiagramaAsync(analiseDiagramaId, processarDiagramaDto.NomeFisico, processarDiagramaDto.LocalizacaoUrl, processarDiagramaDto.Extensao);

            if (resultado.Sucesso)
            {
                processamento.ConcluirProcessamento(
                    resultado.DescricaoAnalise!,
                    resultado.ComponentesIdentificados,
                    resultado.RiscosArquiteturais,
                    resultado.RecomendacoesBasicas,
                    resultado.TentativasRealizadas);
                await gateway.SalvarAsync(processamento);
                await messagePublisher.PublicarDiagramaAnalisadoAsync(processamento);

                var duracaoMs = (long)(DateTimeOffset.UtcNow - inicioProcessamento).TotalMilliseconds;
                metrics.RegistrarProcessamentoConcluido(analiseDiagramaId, duracaoMs);

                logger.ComUseCase(this).ComPropriedade(LogNomesPropriedades.AnaliseDiagramaId, analiseDiagramaId).LogDebug($"Processamento concluído com sucesso para {{{LogNomesPropriedades.AnaliseDiagramaId}}}", analiseDiagramaId);
            }
            else
            {
                var motivo = resultado.MotivoErro ?? "Erro desconhecido na análise do diagrama";
                processamento.RegistrarFalha(resultado.TentativasRealizadas);
                await gateway.SalvarAsync(processamento);
                await messagePublisher.PublicarProcessamentoErroAsync(processamento, motivo);

                metrics.RegistrarProcessamentoFalha(analiseDiagramaId, motivo, resultado.TentativasRealizadas);

                logger.ComUseCase(this).ComPropriedade(LogNomesPropriedades.AnaliseDiagramaId, analiseDiagramaId).ComPropriedade(LogNomesPropriedades.Tentativas, resultado.TentativasRealizadas).LogError($"Falha no processamento {{{LogNomesPropriedades.AnaliseDiagramaId}}} após esgotar as tentativas de análise. {LogNomesPropriedades.Motivo}: {{{LogNomesPropriedades.Motivo}}}. {LogNomesPropriedades.Tentativas}: {{{LogNomesPropriedades.Tentativas}}}", analiseDiagramaId, motivo, resultado.TentativasRealizadas);
            }
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
}
