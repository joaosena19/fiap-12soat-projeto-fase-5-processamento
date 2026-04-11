using Amazon.S3;
using Infrastructure.Armazenamento;

namespace Tests.Infrastructure.Armazenamento;

public class S3ArquivoDownloaderTests
{
    [Fact(DisplayName = "Deve baixar arquivo do S3 a partir da URL informada")]
    [Trait("Infrastructure", "S3ArquivoDownloader")]
    public async Task BaixarArquivoAsync_DeveRetornarBytes_QuandoUrlValida()
    {
        // Arrange
        var amazonS3Mock = new Mock<IAmazonS3>();
        var downloader = new S3ArquivoDownloader(amazonS3Mock.Object, new LoggerFactory());
        var bytesEsperados = new byte[] { 1, 2, 3, 4 };
        await using var stream = new MemoryStream(bytesEsperados);
        amazonS3Mock.AoObterObjeto().Retorna(stream, bucket: "bucket-teste", key: "pasta/arquivo.png");

        // Act
        var resultado = await downloader.BaixarArquivoAsync("https://bucket-teste.s3.amazonaws.com/pasta/arquivo.png");

        // Assert
        resultado.ShouldBe(bytesEsperados);
    }

    [Fact(DisplayName = "Deve propagar exceção quando S3 lança erro")]
    [Trait("Infrastructure", "S3ArquivoDownloader")]
    public async Task BaixarArquivoAsync_DevePropagarExcecao_QuandoS3LancaErro()
    {
        // Arrange
        var amazonS3Mock = new Mock<IAmazonS3>();
        var downloader = new S3ArquivoDownloader(amazonS3Mock.Object, new LoggerFactory());
        amazonS3Mock.AoObterObjeto().LancaExcecao(new AmazonS3Exception("Acesso negado"));

        // Act & Assert
        await Should.ThrowAsync<AmazonS3Exception>(() => downloader.BaixarArquivoAsync("https://bucket-teste.s3.amazonaws.com/pasta/arquivo.png"));
    }
}