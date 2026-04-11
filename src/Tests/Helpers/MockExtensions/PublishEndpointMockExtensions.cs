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

    public static T ObterMensagemPublicada<T>(this Mock<IPublishEndpoint> mock, int indice = 0) where T : class
    {
        mock.Invocations.Count.ShouldBeGreaterThan(indice, $"Esperava ao menos {indice + 1} publicação(ões), mas encontrou {mock.Invocations.Count}");
        return mock.Invocations[indice].Arguments[0].ShouldBeOfType<T>();
    }
}
