using MassTransit;

namespace Tests.Helpers.MockExtensions;

public static class PublishEndpointMockExtensions
{
    public static void AoPublicarNaoFazNada<T>(this Mock<IPublishEndpoint> mock) where T : class
    {
        mock.Setup(x => x.Publish(It.IsAny<T>(), It.IsAny<CancellationToken>())).Returns(Task.CompletedTask);
    }

    public static void DeveTerPublicado<T>(this Mock<IPublishEndpoint> mock) where T : class
    {
        mock.Verify(x => x.Publish(It.IsAny<T>(), It.IsAny<CancellationToken>()), Times.AtLeastOnce);
    }
}
