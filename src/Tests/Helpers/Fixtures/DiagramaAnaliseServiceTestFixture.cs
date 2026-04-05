using Infrastructure.Armazenamento;
using Infrastructure.LLM;
using Microsoft.Extensions.Configuration;

namespace Tests.Helpers.Fixtures;

public class DiagramaAnaliseServiceTestFixture
{
    public Mock<IDiagramaAnaliseClient> ClienteLlmMock { get; }
    public Mock<IArquivoDiagramaDownloader> DownloaderMock { get; }
    public ILoggerFactory LoggerFactory { get; }
    public IConfiguration Configuration { get; }
    public DiagramaAnaliseService Service { get; }

    public DiagramaAnaliseServiceTestFixture(int maxTentativas = 2, int delaySegundos = 0)
    {
        ClienteLlmMock = new Mock<IDiagramaAnaliseClient>();
        DownloaderMock = new Mock<IArquivoDiagramaDownloader>();
        LoggerFactory = new LoggerFactory();

        Configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Resiliencia:LLM:MaxTentativas"] = maxTentativas.ToString(),
                ["Resiliencia:LLM:DelaySegundos"] = delaySegundos.ToString()
            })
            .Build();

        Service = new DiagramaAnaliseService(ClienteLlmMock.Object, DownloaderMock.Object, LoggerFactory, Configuration);
    }
}
