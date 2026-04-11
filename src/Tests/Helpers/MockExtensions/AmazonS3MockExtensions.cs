using Amazon.S3;
using Amazon.S3.Model;

namespace Tests.Helpers.MockExtensions;

public static class AmazonS3MockExtensions
{
    public static S3GetObjectSetup AoObterObjeto(this Mock<IAmazonS3> mock) => new(mock);

    public class S3GetObjectSetup
    {
        private readonly Mock<IAmazonS3> _mock;

        public S3GetObjectSetup(Mock<IAmazonS3> mock) => _mock = mock;

        public void Retorna(Stream responseStream, string? bucket = null, string? key = null)
        {
            if (bucket is not null && key is not null)
                _mock.Setup(x => x.GetObjectAsync(It.Is<GetObjectRequest>(r => r.BucketName == bucket && r.Key == key), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(new GetObjectResponse { ResponseStream = responseStream });
            else
                _mock.Setup(x => x.GetObjectAsync(It.IsAny<GetObjectRequest>(), It.IsAny<CancellationToken>()))
                     .ReturnsAsync(new GetObjectResponse { ResponseStream = responseStream });
        }

        public void LancaExcecao(Exception excecao)
        {
            _mock.Setup(x => x.GetObjectAsync(It.IsAny<GetObjectRequest>(), It.IsAny<CancellationToken>())).ThrowsAsync(excecao);
        }
    }
}
