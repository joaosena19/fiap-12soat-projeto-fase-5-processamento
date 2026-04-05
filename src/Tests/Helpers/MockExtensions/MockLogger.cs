namespace Tests.Helpers.MockExtensions;

public static class MockLogger
{
    public static Mock<IAppLogger> Criar()
    {
        var mock = new Mock<IAppLogger>();
        mock.Setup(x => x.ComPropriedade(It.IsAny<string>(), It.IsAny<object?>())).Returns(mock.Object);
        return mock;
    }

    public static void DeveTerLogadoInformation(this Mock<IAppLogger> mock)
    {
        mock.Verify(x => x.LogInformation(It.IsAny<string>(), It.IsAny<object[]>()), Times.AtLeastOnce);
    }

    public static void DeveTerLogadoDebug(this Mock<IAppLogger> mock)
    {
        mock.Verify(x => x.LogDebug(It.IsAny<string>(), It.IsAny<object[]>()), Times.AtLeastOnce);
    }

    public static void DeveTerLogadoWarning(this Mock<IAppLogger> mock)
    {
        mock.Verify(x => x.LogWarning(It.IsAny<string>(), It.IsAny<object[]>()), Times.AtLeastOnce);
    }

    public static void DeveTerLogadoError(this Mock<IAppLogger> mock)
    {
        mock.Verify(x => x.LogError(It.IsAny<string>(), It.IsAny<object[]>()), Times.AtLeastOnce);
    }

    public static void DeveTerLogadoErrorComException(this Mock<IAppLogger> mock)
    {
        mock.Verify(x => x.LogError(It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<object[]>()), Times.AtLeastOnce);
    }
}
