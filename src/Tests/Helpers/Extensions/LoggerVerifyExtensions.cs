namespace Tests.Helpers.Extensions;

public static class LoggerVerifyExtensions
{
    public static void DeveTerLogadoDebug<T>(this Mock<ILogger<T>> mock, int vezes = 1)
    {
        mock.Verify(x => x.Log(LogLevel.Debug, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), null, It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Exactly(vezes));
    }

    public static void DeveTerLogadoInformation<T>(this Mock<ILogger<T>> mock, int vezes = 1)
    {
        mock.Verify(x => x.Log(LogLevel.Information, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), null, It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Exactly(vezes));
    }

    public static void DeveTerLogadoWarning<T>(this Mock<ILogger<T>> mock, int vezes = 1)
    {
        mock.Verify(x => x.Log(LogLevel.Warning, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), null, It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Exactly(vezes));
    }

    public static void DeveTerLogadoWarningComException<T>(this Mock<ILogger<T>> mock, Exception excecao, int vezes = 1)
    {
        mock.Verify(x => x.Log(LogLevel.Warning, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), excecao, It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Exactly(vezes));
    }

    public static void DeveTerLogadoError<T>(this Mock<ILogger<T>> mock, int vezes = 1)
    {
        mock.Verify(x => x.Log(LogLevel.Error, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), null, It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Exactly(vezes));
    }

    public static void DeveTerLogadoErrorComException<T>(this Mock<ILogger<T>> mock, Exception excecao, int vezes = 1)
    {
        mock.Verify(x => x.Log(LogLevel.Error, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), excecao, It.IsAny<Func<It.IsAnyType, Exception?, string>>()), Times.Exactly(vezes));
    }
}
