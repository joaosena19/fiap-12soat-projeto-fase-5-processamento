using Infrastructure.LLM;
using Microsoft.Extensions.AI;

namespace Tests.Infrastructure.LLM;

public class LlmDiagramaAnaliseClientTestFixture
{
    public Mock<IChatClient> ChatClientMock { get; } = new();
    public LlmDiagramaAnaliseClient Client { get; }

    public LlmDiagramaAnaliseClientTestFixture()
    {
        Client = new LlmDiagramaAnaliseClient(ChatClientMock.Object, new LoggerFactory());
    }

    public async Task<ResultadoAnaliseDto> AnalisarAsync(string nomeArquivo = "arquivo.png", byte[]? conteudo = null, string extensao = ".png") =>
        await Client.AnalisarDiagramaAsync(Guid.NewGuid(), nomeArquivo, conteudo ?? [0x01], extensao);
}
