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

    public static void DeveTerLogadoDebug(this Mock<IAppLogger> mock, string template, int vezes = 1)
    {
        mock.Verify(x => x.LogDebug(template, It.IsAny<object[]>()), Times.Exactly(vezes));
    }

    public static void DeveTerLogadoInformation(this Mock<IAppLogger> mock, string template, int vezes = 1)
    {
        mock.Verify(x => x.LogInformation(template, It.IsAny<object[]>()), Times.Exactly(vezes));
    }

    public static void DeveTerLogadoWarning(this Mock<IAppLogger> mock, string template, int vezes = 1)
    {
        mock.Verify(x => x.LogWarning(template, It.IsAny<object[]>()), Times.Exactly(vezes));
    }

    public static void DeveTerLogadoWarningComException(this Mock<IAppLogger> mock, Exception excecao, string template, int vezes = 1)
    {
        mock.Verify(x => x.LogWarning(excecao, template, It.IsAny<object[]>()), Times.Exactly(vezes));
    }

    public static void DeveTerLogadoError(this Mock<IAppLogger> mock, string template, int vezes = 1)
    {
        mock.Verify(x => x.LogError(template, It.IsAny<object[]>()), Times.Exactly(vezes));
    }

    public static void NaoDeveTerLogadoError(this Mock<IAppLogger> mock)
    {
        mock.Verify(x => x.LogError(It.IsAny<string>(), It.IsAny<object[]>()), Times.Never);
        mock.Verify(x => x.LogError(It.IsAny<Exception>(), It.IsAny<string>(), It.IsAny<object[]>()), Times.Never);
    }

    public static void DeveTerLogadoErrorComException(this Mock<IAppLogger> mock, Exception excecao, string template, int vezes = 1)
    {
        mock.Verify(x => x.LogError(excecao, template, It.IsAny<object[]>()), Times.Exactly(vezes));
    }
}
