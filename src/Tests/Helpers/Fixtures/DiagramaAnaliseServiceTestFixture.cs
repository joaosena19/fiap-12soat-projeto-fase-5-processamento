using Infrastructure.Armazenamento;
using Infrastructure.LLM;
using Microsoft.Extensions.Configuration;

namespace Tests.Helpers.Fixtures;

public class DiagramaAnaliseServiceTestFixture
{
    public Mock<ILlmClientFactory> ClientFactoryMock { get; }
    public Mock<IArquivoDiagramaDownloader> DownloaderMock { get; }
    public ILoggerFactory LoggerFactory { get; }
    public IConfiguration Configuration { get; }
    public DiagramaAnaliseService Service { get; }

    public DiagramaAnaliseServiceTestFixture(int maxTentativasPorModelo = 2, int delaySegundos = 0, List<string>? modelos = null)
    {
        ClientFactoryMock = new Mock<ILlmClientFactory>();
        DownloaderMock = new Mock<IArquivoDiagramaDownloader>();
        LoggerFactory = new LoggerFactory();

        var opcoes = new LlmOptions
        {
            ApiKey = "api-key-teste",
            Modelos = modelos ?? ["modelo-principal", "modelo-fallback"]
        };

        Configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Resiliencia:LLM:MaxTentativasPorModelo"] = maxTentativasPorModelo.ToString(),
                ["Resiliencia:LLM:DelaySegundos"] = delaySegundos.ToString()
            })
            .Build();

        Service = new DiagramaAnaliseService(opcoes, ClientFactoryMock.Object, DownloaderMock.Object, LoggerFactory, Configuration);
    }
}
