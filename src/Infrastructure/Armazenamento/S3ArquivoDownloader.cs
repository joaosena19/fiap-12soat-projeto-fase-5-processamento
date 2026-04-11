using Amazon.S3;
using Amazon.S3.Model;
using Application.Contracts.Monitoramento;
using Infrastructure.Monitoramento;
using Microsoft.Extensions.Logging;
using Shared.Constants;

namespace Infrastructure.Armazenamento;

public class S3ArquivoDownloader : IArquivoDiagramaDownloader
{
    private readonly IAmazonS3 _amazonS3;
    private readonly IAppLogger _logger;

    public S3ArquivoDownloader(IAmazonS3 amazonS3, ILoggerFactory loggerFactory)
    {
        _amazonS3 = amazonS3;
        _logger = loggerFactory.CriarAppLogger<S3ArquivoDownloader>();
    }

    public async Task<byte[]> BaixarArquivoAsync(string localizacaoUrl)
    {
        var uri = new Uri(localizacaoUrl);
        var hostPartes = uri.Host.Split('.');

        if (hostPartes.Length == 0)
            throw new InvalidOperationException("URL do S3 inválida.");

        var bucketName = hostPartes[0];
        var key = uri.AbsolutePath.TrimStart('/');

        try
        {
            var resposta = await _amazonS3.GetObjectAsync(new GetObjectRequest { BucketName = bucketName, Key = key });

            using var memoryStream = new MemoryStream();
            await resposta.ResponseStream.CopyToAsync(memoryStream);
            return memoryStream.ToArray();
        }
        catch (AmazonS3Exception ex)
        {
            _logger.ComPropriedade(LogNomesPropriedades.Bucket, bucketName)
                   .ComPropriedade(LogNomesPropriedades.Key, key)
                   .LogError(ex, "Falha ao baixar arquivo do S3. StatusCode: {StatusCode}", (int)ex.StatusCode);
            throw;
        }
    }
}