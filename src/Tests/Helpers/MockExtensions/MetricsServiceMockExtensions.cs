namespace Tests.Helpers.MockExtensions;

public static class MetricsServiceMockExtensions
{
    public static void DeveTerRegistradoProcessamentoIniciado(this Mock<IMetricsService> mock)
    {
        mock.Verify(x => x.RegistrarProcessamentoIniciado(It.IsAny<Guid>()), Times.Once);
    }

    public static void DeveTerRegistradoProcessamentoConcluido(this Mock<IMetricsService> mock)
    {
        mock.Verify(x => x.RegistrarProcessamentoConcluido(It.IsAny<Guid>(), It.IsAny<long>()), Times.Once);
    }

    public static void NaoDeveTerRegistradoProcessamentoConcluido(this Mock<IMetricsService> mock)
    {
        mock.Verify(x => x.RegistrarProcessamentoConcluido(It.IsAny<Guid>(), It.IsAny<long>()), Times.Never);
    }

    public static void DeveTerRegistradoProcessamentoFalha(this Mock<IMetricsService> mock)
    {
        mock.Verify(x => x.RegistrarProcessamentoFalha(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<int>()), Times.AtLeastOnce);
    }

    public static void NaoDeveTerRegistradoProcessamentoFalha(this Mock<IMetricsService> mock)
    {
        mock.Verify(x => x.RegistrarProcessamentoFalha(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<int>()), Times.Never);
    }

    public static void DeveTerRegistradoProcessamentoRejeitado(this Mock<IMetricsService> mock)
    {
        mock.Verify(x => x.RegistrarProcessamentoRejeitado(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<int>()), Times.AtLeastOnce);
    }

    public static void NaoDeveTerRegistradoProcessamentoRejeitado(this Mock<IMetricsService> mock)
    {
        mock.Verify(x => x.RegistrarProcessamentoRejeitado(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<int>()), Times.Never);
    }
}
