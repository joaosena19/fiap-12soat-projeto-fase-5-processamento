using Infrastructure.LLM;

namespace Tests.Helpers.MockExtensions;

public static class LlmClientFactoryMockExtensions
{
    public static CriarParaSetup AoCriarPara(this Mock<ILlmClientFactory> mock, string? modelo = null) => new(mock, modelo);

    public static void DeveTerCriadoPara(this Mock<ILlmClientFactory> mock, string modelo) =>
        mock.Verify(x => x.CriarPara(modelo), Times.AtLeastOnce);

    public static void NaoDeveTerCriadoPara(this Mock<ILlmClientFactory> mock, string modelo) =>
        mock.Verify(x => x.CriarPara(modelo), Times.Never);

    public class CriarParaSetup
    {
        private readonly Mock<ILlmClientFactory> _mock;
        private readonly string? _modelo;

        public CriarParaSetup(Mock<ILlmClientFactory> mock, string? modelo)
        {
            _mock = mock;
            _modelo = modelo;
        }

        public Mock<IDiagramaAnaliseClient> RetornaMock()
        {
            var clientMock = new Mock<IDiagramaAnaliseClient>();
            if (_modelo != null)
                _mock.Setup(x => x.CriarPara(_modelo)).Returns(clientMock.Object);
            else
                _mock.Setup(x => x.CriarPara(It.IsAny<string>())).Returns(clientMock.Object);

            return clientMock;
        }

        public void Retorna(IDiagramaAnaliseClient client)
        {
            if (_modelo != null)
                _mock.Setup(x => x.CriarPara(_modelo)).Returns(client);
            else
                _mock.Setup(x => x.CriarPara(It.IsAny<string>())).Returns(client);
        }
    }
}
