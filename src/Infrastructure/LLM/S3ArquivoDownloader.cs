using Amazon.S3;
using Application.Contracts.Monitoramento;
using Infrastructure.Monitoramento;
using Microsoft.Extensions.Logging;
using Shared.Constants;

namespace Infrastructure.LLM;

/// <summary>
/// Realiza download de arquivos do S3 para envio à LLM.
/// </summary>
public class S3ArquivoDownloader
{
    private readonly IAmazonS3 _s3Client;
    private readonly IAppLogger _logger;

    public S3ArquivoDownloader(IAmazonS3 s3Client, ILoggerFactory loggerFactory)
    {
        _s3Client = s3Client;
        _logger = new LoggerAdapter<S3ArquivoDownloader>(loggerFactory.CreateLogger<S3ArquivoDownloader>());
    }

    public async Task<byte[]> BaixarArquivoAsync(string localizacaoUrl)
    {
        var (bucket, key) = ParseS3Url(localizacaoUrl);

        try
        {
            var response = await _s3Client.GetObjectAsync(bucket, key);

            using var memoryStream = new MemoryStream();
            await response.ResponseStream.CopyToAsync(memoryStream);
            return memoryStream.ToArray();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, $"Falha ao baixar arquivo do S3. {{{LogNomesPropriedades.Bucket}}}/{{{LogNomesPropriedades.Key}}}", bucket, key);
            throw;
        }
    }

    private static (string bucket, string key) ParseS3Url(string url)
    {
        if (!url.StartsWith("s3://"))
            throw new InvalidOperationException($"URL de S3 inválida: {url}");

        var semProtocolo = url["s3://".Length..];
        var separador = semProtocolo.IndexOf('/');

        if (separador < 0)
            throw new InvalidOperationException($"URL de S3 sem key: {url}");

        var bucket = semProtocolo[..separador];
        var key = semProtocolo[(separador + 1)..];
        return (bucket, key);
    }
}
