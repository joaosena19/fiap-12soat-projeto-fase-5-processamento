using Infrastructure.Armazenamento;

namespace Tests.Helpers.MockExtensions;

public static class ArquivoDiagramaDownloaderMockExtensions
{
    public static DownloaderSetup AoBaixar(this Mock<IArquivoDiagramaDownloader> mock) => new(mock);

    public class DownloaderSetup
    {
        private readonly Mock<IArquivoDiagramaDownloader> _mock;

        public DownloaderSetup(Mock<IArquivoDiagramaDownloader> mock) => _mock = mock;

        public void Retorna(byte[] bytes)
        {
            _mock.Setup(x => x.BaixarArquivoAsync(It.IsAny<string>())).ReturnsAsync(bytes);
        }

        public void LancaExcecao(Exception excecao)
        {
            _mock.Setup(x => x.BaixarArquivoAsync(It.IsAny<string>())).ThrowsAsync(excecao);
        }
    }
}
