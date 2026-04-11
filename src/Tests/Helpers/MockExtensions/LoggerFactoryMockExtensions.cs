using Microsoft.Extensions.Logging;

namespace Tests.Helpers.MockExtensions;

public static class LoggerFactoryMockExtensions
{
    public static Mock<ILoggerFactory> CriarLoggerFactoryMock()
    {
        var loggerFactoryMock = new Mock<ILoggerFactory>();
        loggerFactoryMock.Setup(x => x.CreateLogger(It.IsAny<string>())).Returns(new Mock<ILogger>().Object);
        return loggerFactoryMock;
    }
}
