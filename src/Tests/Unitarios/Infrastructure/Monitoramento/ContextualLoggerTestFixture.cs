using Infrastructure.Monitoramento;

namespace Tests.Infrastructure.Monitoramento;

public class ContextualLoggerTestFixture
{
    public Mock<IAppLogger> InnerMock { get; } = new();
    public ContextualLogger Logger { get; }

    public ContextualLoggerTestFixture(Dictionary<string, object?>? propriedades = null)
    {
        InnerMock.Setup(x => x.ComPropriedade(It.IsAny<string>(), It.IsAny<object?>())).Returns(InnerMock.Object);
        Logger = new ContextualLogger(InnerMock.Object, propriedades ?? new Dictionary<string, object?> { ["key"] = "value" });
    }
}
