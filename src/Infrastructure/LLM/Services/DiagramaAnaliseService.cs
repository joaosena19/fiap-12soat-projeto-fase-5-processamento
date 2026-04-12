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
    private readonly IDiagramaAnaliseClient _client;
    private readonly IArquivoDiagramaDownloader _arquivoDiagramaDownloader;
    private readonly IAppLogger _logger;
    private readonly ResiliencePipeline<ResultadoAnaliseDto> _pipeline;

    public DiagramaAnaliseService(IDiagramaAnaliseClient client, IArquivoDiagramaDownloader arquivoDiagramaDownloader, ILoggerFactory loggerFactory, IConfiguration configuration)
    {
        _client = client;
        _arquivoDiagramaDownloader = arquivoDiagramaDownloader;
        _logger = loggerFactory.CriarAppLogger<DiagramaAnaliseService>();
        _pipeline = ResilienciaAnaliseDiagramaPipelineFactory.Criar(ResilienciaAnaliseDiagramaOptions.Criar(configuration), loggerFactory.CreateLogger<DiagramaAnaliseService>());
    }

    public async Task<ResultadoAnaliseDto> AnalisarDiagramaAsync(Guid analiseDiagramaId, string nomeFisico, string localizacaoUrl, string extensao)
    {
        var tentativasRealizadas = 0;
        var cronometro = Stopwatch.StartNew();

        try
        {
            _logger.ComPropriedade(LogNomesPropriedades.AnaliseDiagramaId, analiseDiagramaId).LogDebug("Iniciando download do diagrama de {LocalizacaoUrl} para {AnaliseDiagramaId}", localizacaoUrl, analiseDiagramaId);

            var conteudoArquivo = await BaixarConteudoArquivoAsync(localizacaoUrl);

            _logger.ComPropriedade(LogNomesPropriedades.AnaliseDiagramaId, analiseDiagramaId)
                   .ComPropriedade(LogNomesPropriedades.Tamanho, conteudoArquivo.Length)
                   .LogDebug("Download concluído para {AnaliseDiagramaId}. Tamanho: {Tamanho} bytes", analiseDiagramaId, conteudoArquivo.Length);

            var resultado = await ExecutarAnaliseComResilienciaAsync(analiseDiagramaId, nomeFisico, conteudoArquivo, extensao, tentativas => tentativasRealizadas = tentativas);

            cronometro.Stop();
            _logger.ComPropriedade(LogNomesPropriedades.AnaliseDiagramaId, analiseDiagramaId)
                   .ComPropriedade(LogNomesPropriedades.DuracaoMs, cronometro.ElapsedMilliseconds)
                   .ComPropriedade(LogNomesPropriedades.Tentativas, tentativasRealizadas)
                   .LogInformation("Análise LLM concluída para {AnaliseDiagramaId} em {DuracaoMs}ms após {Tentativas} tentativa(s). Sucesso: {Sucesso}", analiseDiagramaId, cronometro.ElapsedMilliseconds, tentativasRealizadas, resultado.Sucesso);

            return resultado;
        }
        catch (LlmPermanentException ex)
        {
            cronometro.Stop();
            _logger.ComPropriedade(LogNomesPropriedades.AnaliseDiagramaId, analiseDiagramaId)
                   .ComPropriedade(LogNomesPropriedades.DuracaoMs, cronometro.ElapsedMilliseconds)
                   .ComPropriedade(LogNomesPropriedades.Tentativas, tentativasRealizadas)
                   .LogError(ex, "Falha permanente na LLM para {AnaliseDiagramaId} após {Tentativas} tentativa(s) em {DuracaoMs}ms. Motivo: {Motivo}", analiseDiagramaId, tentativasRealizadas, cronometro.ElapsedMilliseconds, ex.Message);
            return CriarResultadoFalha(ex, tentativasRealizadas);
        }
        catch (Exception ex)
        {
            cronometro.Stop();
            _logger.ComPropriedade(LogNomesPropriedades.AnaliseDiagramaId, analiseDiagramaId)
                   .ComPropriedade(LogNomesPropriedades.DuracaoMs, cronometro.ElapsedMilliseconds)
                   .ComPropriedade(LogNomesPropriedades.Tentativas, tentativasRealizadas)
                   .LogError(ex, "Falha ao analisar diagrama na LLM para {AnaliseDiagramaId} após {Tentativas} tentativa(s) em {DuracaoMs}ms. ExceptionType: {ExceptionType}, Motivo: {Motivo}", analiseDiagramaId, tentativasRealizadas, cronometro.ElapsedMilliseconds, ex.GetType().FullName ?? ex.GetType().Name, ex.Message);
            return CriarResultadoFalha(ex, tentativasRealizadas);
        }
    }

    private async Task<byte[]> BaixarConteudoArquivoAsync(string localizacaoUrl)
    {
        return await _arquivoDiagramaDownloader.BaixarArquivoAsync(localizacaoUrl);
    }

    private async Task<ResultadoAnaliseDto> ExecutarAnaliseComResilienciaAsync(Guid analiseDiagramaId, string nomeFisico, byte[] conteudoArquivo, string extensao, Action<int> atualizarTentativas)
    {
        var tentativasRealizadas = 0;

        var resultado = await _pipeline.ExecuteAsync(async cancellationToken =>
        {
            tentativasRealizadas++;
            atualizarTentativas(tentativasRealizadas);

            if (tentativasRealizadas > 1)
                _logger.LogWarning($"Nova tentativa de análise do diagrama para {{{LogNomesPropriedades.AnaliseDiagramaId}}}. {{{LogNomesPropriedades.Tentativas}}}", analiseDiagramaId, tentativasRealizadas);

            return await _client.AnalisarDiagramaAsync(analiseDiagramaId, nomeFisico, conteudoArquivo, extensao);
        }, CancellationToken.None);

        return resultado with { TentativasRealizadas = tentativasRealizadas };
    }

    private static ResultadoAnaliseDto CriarResultadoFalha(Exception ex, int tentativasRealizadas)
    {
        return new ResultadoAnaliseDto
        {
            Sucesso = false,
            MotivoErro = ex.Message,
            TentativasRealizadas = tentativasRealizadas
        };
    }
}