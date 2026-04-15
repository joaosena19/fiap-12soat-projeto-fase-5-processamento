using Application.Contracts.LLM;
using Application.Contracts.Monitoramento;
using Infrastructure.Armazenamento;
using Infrastructure.Monitoramento;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Polly;
using Shared.Constants;
using System.Diagnostics;

namespace Infrastructure.LLM;

public class DiagramaAnaliseService : IDiagramaAnaliseService
{
    private readonly LlmOptions _opcoes;
    private readonly ILlmClientFactory _clientFactory;
    private readonly IArquivoDiagramaDownloader _arquivoDiagramaDownloader;
    private readonly IAppLogger _logger;
    private readonly ILoggerFactory _loggerFactory;
    private readonly ResiliencePipeline<ResultadoAnaliseDto> _pipeline;

    public DiagramaAnaliseService(LlmOptions opcoes, ILlmClientFactory clientFactory, IArquivoDiagramaDownloader arquivoDiagramaDownloader, ILoggerFactory loggerFactory, IConfiguration configuration)
    {
        _opcoes = opcoes;
        _clientFactory = clientFactory;
        _arquivoDiagramaDownloader = arquivoDiagramaDownloader;
        _loggerFactory = loggerFactory;
        _logger = loggerFactory.CriarAppLogger<DiagramaAnaliseService>();
        _pipeline = ResilienciaAnaliseDiagramaPipelineFactory.Criar(ResilienciaAnaliseDiagramaOptions.Criar(configuration), loggerFactory.CreateLogger<DiagramaAnaliseService>());
    }

    public async Task<ResultadoAnaliseDto> AnalisarDiagramaAsync(Guid analiseDiagramaId, string nomeFisico, string localizacaoUrl, string extensao)
    {
        var cronometro = Stopwatch.StartNew();

        if (string.IsNullOrWhiteSpace(localizacaoUrl))
        {
            cronometro.Stop();
            _logger.ComPropriedade(LogNomesPropriedades.AnaliseDiagramaId, analiseDiagramaId).LogError("LocalizacaoUrl vazia ou nula para {AnaliseDiagramaId}. A mensagem pode ter sido enviada com dados incompletos.", analiseDiagramaId);
            return CriarResultadoFalha(new InvalidOperationException("LocalizacaoUrl não pode ser vazia."), 0, OrigemErroConstantes.Armazenamento);
        }

        byte[] conteudoArquivo;
        try
        {
            _logger.ComPropriedade(LogNomesPropriedades.AnaliseDiagramaId, analiseDiagramaId).LogDebug("Iniciando download do diagrama de {LocalizacaoUrl} para {AnaliseDiagramaId}", localizacaoUrl, analiseDiagramaId);
            conteudoArquivo = await BaixarConteudoArquivoAsync(localizacaoUrl);
            _logger.ComPropriedade(LogNomesPropriedades.AnaliseDiagramaId, analiseDiagramaId)
                   .ComPropriedade(LogNomesPropriedades.Tamanho, conteudoArquivo.Length)
                   .LogDebug("Download concluído para {AnaliseDiagramaId}. Tamanho: {Tamanho} bytes", analiseDiagramaId, conteudoArquivo.Length);
        }
        catch (Exception ex)
        {
            cronometro.Stop();
            _logger.ComPropriedade(LogNomesPropriedades.AnaliseDiagramaId, analiseDiagramaId)
                   .ComPropriedade(LogNomesPropriedades.DuracaoMs, cronometro.ElapsedMilliseconds)
                   .LogError(ex, "Falha ao baixar arquivo do armazenamento para {AnaliseDiagramaId} em {DuracaoMs}ms. ExceptionType: {ExceptionType}, Motivo: {Motivo}", analiseDiagramaId, cronometro.ElapsedMilliseconds, ex.GetType().FullName ?? ex.GetType().Name, ex.Message);
            return CriarResultadoFalha(ex, 0, OrigemErroConstantes.Armazenamento);
        }

        var resultado = await TentarAnalisarComFallbackAsync(analiseDiagramaId, nomeFisico, conteudoArquivo, extensao);

        cronometro.Stop();
        _logger.ComPropriedade(LogNomesPropriedades.AnaliseDiagramaId, analiseDiagramaId)
               .ComPropriedade(LogNomesPropriedades.DuracaoMs, cronometro.ElapsedMilliseconds)
               .ComPropriedade(LogNomesPropriedades.Tentativas, resultado.TentativasRealizadas)
               .LogInformation("Análise LLM concluída para {AnaliseDiagramaId} em {DuracaoMs}ms após {Tentativas} tentativa(s). Sucesso: {Sucesso}", analiseDiagramaId, cronometro.ElapsedMilliseconds, resultado.TentativasRealizadas, resultado.Sucesso);

        return resultado;
    }

    private async Task<ResultadoAnaliseDto> TentarAnalisarComFallbackAsync(Guid analiseDiagramaId, string nomeFisico, byte[] conteudoArquivo, string extensao)
    {
        var modelos = _opcoes.Modelos;
        var tentativasTotal = 0;

        for (var i = 0; i < modelos.Count; i++)
        {
            var modelo = modelos[i];
            var client = _clientFactory.CriarPara(modelo);
            var tentativasModelo = 0;

            _logger.ComPropriedade(LogNomesPropriedades.AnaliseDiagramaId, analiseDiagramaId)
                   .LogInformation("Tentando modelo {Modelo} ({Indice}/{Total}) para {AnaliseDiagramaId}.", modelo, i + 1, modelos.Count, analiseDiagramaId);

            try
            {
                var resultado = await ExecutarAnaliseComResilienciaAsync(analiseDiagramaId, nomeFisico, conteudoArquivo, extensao, client, tentativas => tentativasModelo = tentativas);
                tentativasTotal += tentativasModelo;
                return resultado with { TentativasRealizadas = tentativasTotal };
            }
            catch (LlmIndisponivelException ex)
            {
                tentativasTotal += Math.Max(tentativasModelo, 1);

                _logger.ComPropriedade(LogNomesPropriedades.AnaliseDiagramaId, analiseDiagramaId)
                       .LogWarning("Modelo {Modelo} indisponível (HTTP {CodigoHttp}) para {AnaliseDiagramaId}.", modelo, ex.CodigoHttp, analiseDiagramaId);

                if (i < modelos.Count - 1)
                    continue;

                _logger.ComPropriedade(LogNomesPropriedades.AnaliseDiagramaId, analiseDiagramaId)
                       .LogError(ex, "Todos os modelos LLM estão indisponíveis para {AnaliseDiagramaId} após {Tentativas} tentativa(s).", analiseDiagramaId, tentativasTotal);
                return CriarResultadoFalha(ex, tentativasTotal, OrigemErroConstantes.Llm);
            }
            catch (LlmPermanentException ex)
            {
                tentativasTotal += Math.Max(tentativasModelo, 1);

                _logger.ComPropriedade(LogNomesPropriedades.AnaliseDiagramaId, analiseDiagramaId)
                       .LogError(ex, "Falha permanente na LLM para {AnaliseDiagramaId} após {Tentativas} tentativa(s).", analiseDiagramaId, tentativasTotal);
                return CriarResultadoFalha(ex, tentativasTotal, OrigemErroConstantes.Llm);
            }
            catch (LlmTransientException ex)
            {
                tentativasTotal += tentativasModelo;

                _logger.ComPropriedade(LogNomesPropriedades.AnaliseDiagramaId, analiseDiagramaId)
                       .LogWarning(ex, "Modelo {Modelo} esgotou retries para {AnaliseDiagramaId}.", modelo, analiseDiagramaId);

                if (i < modelos.Count - 1)
                    continue;

                _logger.ComPropriedade(LogNomesPropriedades.AnaliseDiagramaId, analiseDiagramaId)
                       .LogError(ex, "Todos os modelos LLM falharam para {AnaliseDiagramaId} após {Tentativas} tentativa(s).", analiseDiagramaId, tentativasTotal);
                return CriarResultadoFalha(ex, tentativasTotal, OrigemErroConstantes.Llm);
            }
            catch (Exception ex)
            {
                tentativasTotal += Math.Max(tentativasModelo, 1);

                _logger.ComPropriedade(LogNomesPropriedades.AnaliseDiagramaId, analiseDiagramaId)
                       .LogError(ex, "Falha inesperada ao analisar diagrama para {AnaliseDiagramaId}. ExceptionType: {ExceptionType}", analiseDiagramaId, ex.GetType().FullName ?? ex.GetType().Name);
                return CriarResultadoFalha(ex, tentativasTotal, OrigemErroConstantes.Desconhecido);
            }
        }

        _logger.LogError("Nenhum modelo LLM configurado para processar a análise.");
        return CriarResultadoFalha(new InvalidOperationException("Nenhum modelo LLM disponível."), tentativasTotal, OrigemErroConstantes.Llm);
    }

    private async Task<byte[]> BaixarConteudoArquivoAsync(string localizacaoUrl)
    {
        return await _arquivoDiagramaDownloader.BaixarArquivoAsync(localizacaoUrl);
    }

    private async Task<ResultadoAnaliseDto> ExecutarAnaliseComResilienciaAsync(Guid analiseDiagramaId, string nomeFisico, byte[] conteudoArquivo, string extensao, IDiagramaAnaliseClient client, Action<int> atualizarTentativas)
    {
        var tentativasRealizadas = 0;

        var resultado = await _pipeline.ExecuteAsync(async cancellationToken =>
        {
            tentativasRealizadas++;
            atualizarTentativas(tentativasRealizadas);

            if (tentativasRealizadas > 1)
                _logger.LogWarning($"Nova tentativa de análise do diagrama para {{{LogNomesPropriedades.AnaliseDiagramaId}}}. {{{LogNomesPropriedades.Tentativas}}}", analiseDiagramaId, tentativasRealizadas);

            return await client.AnalisarDiagramaAsync(analiseDiagramaId, nomeFisico, conteudoArquivo, extensao);
        }, CancellationToken.None);

        return resultado with { TentativasRealizadas = tentativasRealizadas };
    }

    private static ResultadoAnaliseDto CriarResultadoFalha(Exception ex, int tentativasRealizadas, string? origemErro = null)
    {
        return new ResultadoAnaliseDto
        {
            Sucesso = false,
            MotivoErro = ex.Message,
            TentativasRealizadas = tentativasRealizadas,
            OrigemErro = origemErro
        };
    }
}