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
        _pipeline = ResilienciaAnaliseDiagramaPipelineFactory.Criar(ResilienciaAnaliseDiagramaOptions.Criar(configuration));
    }

    public async Task<ResultadoAnaliseDto> AnalisarDiagramaAsync(Guid analiseDiagramaId, string nomeFisico, string localizacaoUrl, string extensao)
    {
        var tentativasRealizadas = 0;
        var cronometro = Stopwatch.StartNew();

        try
        {
            var conteudoArquivo = await BaixarConteudoArquivoAsync(localizacaoUrl);
            var resultado = await ExecutarAnaliseComResilienciaAsync(analiseDiagramaId, nomeFisico, conteudoArquivo, extensao, tentativas => tentativasRealizadas = tentativas);

            _logger.ComPropriedade(LogNomesPropriedades.AnaliseDiagramaId, analiseDiagramaId)
                   .ComPropriedade(LogNomesPropriedades.DuracaoMs, cronometro.ElapsedMilliseconds)
                   .LogDebug($"Análise LLM concluída para {{{LogNomesPropriedades.AnaliseDiagramaId}}} em {{{LogNomesPropriedades.DuracaoMs}}}ms", analiseDiagramaId, cronometro.ElapsedMilliseconds);

            return resultado;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Falha ao analisar diagrama na LLM para {{{LogNomesPropriedades.AnaliseDiagramaId}}} após {{{LogNomesPropriedades.Tentativas}}} tentativa(s)", analiseDiagramaId, tentativasRealizadas);
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