using Microsoft.Extensions.AI;

namespace Tests.Helpers.MockExtensions;

public static class ChatClientMockExtensions
{
    public static ChatSetup AoObterResposta(this Mock<IChatClient> mock) => new(mock);

    public class ChatSetup
    {
        private readonly Mock<IChatClient> _mock;

        public ChatSetup(Mock<IChatClient> mock) => _mock = mock;

        public void Retorna(string jsonContent)
        {
            var response = new ChatResponse([new ChatMessage(ChatRole.Assistant, jsonContent)]);
            _mock.Setup(x => x.GetResponseAsync(It.IsAny<IEnumerable<ChatMessage>>(), It.IsAny<ChatOptions?>(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync(response);
        }

        public void RetornaVazio()
        {
            _mock.Setup(x => x.GetResponseAsync(It.IsAny<IEnumerable<ChatMessage>>(), It.IsAny<ChatOptions?>(), It.IsAny<CancellationToken>()))
                 .ReturnsAsync(new ChatResponse());
        }

        public void LancaExcecao(Exception excecao)
        {
            _mock.Setup(x => x.GetResponseAsync(It.IsAny<IEnumerable<ChatMessage>>(), It.IsAny<ChatOptions?>(), It.IsAny<CancellationToken>()))
                 .ThrowsAsync(excecao);
        }
    }
}
