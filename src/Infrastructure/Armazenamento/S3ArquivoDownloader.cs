using Amazon.S3;
using Amazon.S3.Model;

namespace Infrastructure.Armazenamento;

public class S3ArquivoDownloader : IArquivoDiagramaDownloader
{
    private readonly IAmazonS3 _amazonS3;

    public S3ArquivoDownloader(IAmazonS3 amazonS3)
    {
        _amazonS3 = amazonS3;
    }

    public async Task<byte[]> BaixarArquivoAsync(string localizacaoUrl)
    {
        var uri = new Uri(localizacaoUrl);
        var hostPartes = uri.Host.Split('.');

        if (hostPartes.Length == 0)
            throw new InvalidOperationException("URL do S3 inválida.");

        var bucketName = hostPartes[0];
        var key = uri.AbsolutePath.TrimStart('/');
        var resposta = await _amazonS3.GetObjectAsync(new GetObjectRequest { BucketName = bucketName, Key = key });

        using var memoryStream = new MemoryStream();
        await resposta.ResponseStream.CopyToAsync(memoryStream);
        return memoryStream.ToArray();
    }
}