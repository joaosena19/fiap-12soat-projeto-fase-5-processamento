namespace Tests.Helpers.MockExtensions;

public static class DiagramaAnaliseServiceMockExtensions
{
    public static AnalisarSetup AoAnalisar(this Mock<IDiagramaAnaliseService> mock) => new(mock);

    public static void DeveTerAnalisado(this Mock<IDiagramaAnaliseService> mock)
    {
        mock.Verify(x => x.AnalisarDiagramaAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
    }

    public static void NaoDeveTerAnalisado(this Mock<IDiagramaAnaliseService> mock)
    {
        mock.Verify(x => x.AnalisarDiagramaAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    public class AnalisarSetup
    {
        private readonly Mock<IDiagramaAnaliseService> _mock;

        public AnalisarSetup(Mock<IDiagramaAnaliseService> mock) => _mock = mock;

        public void Retorna(ResultadoAnaliseDto resultado)
        {
            _mock.Setup(x => x.AnalisarDiagramaAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(resultado);
        }

        public void LancaExcecao(Exception ex)
        {
            _mock.Setup(x => x.AnalisarDiagramaAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ThrowsAsync(ex);
        }
    }
}
