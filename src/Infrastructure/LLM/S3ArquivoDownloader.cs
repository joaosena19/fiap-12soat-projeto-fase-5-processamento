using Amazon.S3;

namespace Infrastructure.LLM;

/// <summary>
/// Realiza download de arquivos do S3 para envio à LLM.
/// </summary>
public class S3ArquivoDownloader
{
    private readonly IAmazonS3 _s3Client;

    public S3ArquivoDownloader(IAmazonS3 s3Client)
    {
        _s3Client = s3Client;
    }

    public async Task<byte[]> BaixarArquivoAsync(string localizacaoUrl)
    {
        var (bucket, key) = ParseS3Url(localizacaoUrl);

        var response = await _s3Client.GetObjectAsync(bucket, key);

        using var memoryStream = new MemoryStream();
        await response.ResponseStream.CopyToAsync(memoryStream);
        return memoryStream.ToArray();
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
