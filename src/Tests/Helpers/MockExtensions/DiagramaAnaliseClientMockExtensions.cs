using Infrastructure.LLM;

namespace Tests.Helpers.MockExtensions;

public static class DiagramaAnaliseClientMockExtensions
{
    public static AnaliseSetup AoAnalisar(this Mock<IDiagramaAnaliseClient> mock) => new(mock);

    public class AnaliseSetup
    {
        private readonly Mock<IDiagramaAnaliseClient> _mock;

        public AnaliseSetup(Mock<IDiagramaAnaliseClient> mock) => _mock = mock;

        public void Retorna(ResultadoAnaliseDto resultado)
        {
            _mock.Setup(x => x.AnalisarDiagramaAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<string>())).ReturnsAsync(resultado);
        }

        public void LancaExcecao(Exception excecao)
        {
            _mock.Setup(x => x.AnalisarDiagramaAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<byte[]>(), It.IsAny<string>())).ThrowsAsync(excecao);
        }
    }
}
