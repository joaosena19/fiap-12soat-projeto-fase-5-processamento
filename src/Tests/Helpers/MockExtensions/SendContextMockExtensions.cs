using MassTransit;

namespace Tests.Helpers.MockExtensions;

public static class SendContextMockExtensions
{
    public static (Mock<SendContext<T>> sendContextMock, Mock<SendHeaders> headersMock) CriarSendContext<T>() where T : class
    {
        var headersMock = new Mock<SendHeaders>();
        var sendContextMock = new Mock<SendContext<T>>();
        sendContextMock.Setup(x => x.Headers).Returns(headersMock.Object);
        return (sendContextMock, headersMock);
    }

    public static (Mock<SendContext<T>> sendContextMock, Mock<SendHeaders> headersMock) CriarSendContextComCorrelationId<T>() where T : class
    {
        var (sendContextMock, headersMock) = CriarSendContext<T>();
        sendContextMock.SetupProperty(x => x.CorrelationId);
        return (sendContextMock, headersMock);
    }

    public static (Mock<ConsumeContext<T>> contexto, Mock<Headers> headers) CriarConsumeContext<T>(T mensagem) where T : class
    {
        var headers = new Mock<Headers>();
        var contexto = new Mock<ConsumeContext<T>>();
        contexto.SetupGet(x => x.Headers).Returns(headers.Object);
        contexto.SetupGet(x => x.Message).Returns(mensagem);
        return (contexto, headers);
    }

    public static void ComHeader(this Mock<Headers> headers, string nome, string valor)
    {
        object? obj = valor;
        headers.Setup(x => x.TryGetHeader(nome, out obj)).Returns(true);
    }

    public static void SemHeader(this Mock<Headers> headers, string nome)
    {
        object? obj = null;
        headers.Setup(x => x.TryGetHeader(nome, out obj)).Returns(false);
    }

    public static void DeveTerDefinidoHeader(this Mock<SendHeaders> headers, string nome, string valor)
    {
        headers.Verify(x => x.Set(nome, valor), Times.Once);
    }

    public static void DeveTerSidoChamado<T>(this Mock<IPipe<T>> pipe, T contexto) where T : class, PipeContext
    {
        pipe.Verify(x => x.Send(contexto), Times.Once);
    }

    public static void AoEnviar<T>(this Mock<IPipe<T>> pipe, T contexto) where T : class, PipeContext
        => pipe.Setup(x => x.Send(contexto)).Returns(Task.CompletedTask);

    public static void AoEnviarCapturando<T>(this Mock<IPipe<T>> pipe, T contexto, Action callback) where T : class, PipeContext
        => pipe.Setup(x => x.Send(contexto)).Callback(callback).Returns(Task.CompletedTask);

    public static (Mock<PublishContext<T>> publishContextMock, Mock<SendHeaders> headersMock) CriarPublishContextComCorrelationId<T>() where T : class
    {
        var headersMock = new Mock<SendHeaders>();
        var publishContextMock = new Mock<PublishContext<T>>();
        publishContextMock.Setup(x => x.Headers).Returns(headersMock.Object);
        publishContextMock.SetupProperty(x => x.CorrelationId);
        return (publishContextMock, headersMock);
    }

    public static void ComCorrelationId<T>(this Mock<ConsumeContext<T>> contexto, Guid? correlationId) where T : class
        => contexto.SetupGet(x => x.CorrelationId).Returns(correlationId);

    public static Func<string?> CapturarSet(this Mock<SendHeaders> headers, string nome)
    {
        string? valor = null;
        headers.Setup(x => x.Set(nome, It.IsAny<string>()))
               .Callback<string, object>((_, v) => valor = v?.ToString());
        return () => valor;
    }
}
