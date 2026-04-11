using MassTransit;

namespace Tests.Helpers.Extensions;

public static class ProbeContextAssertionExtensions
{
    public static void DeveResponderAoProbeComScopeFilters(Action<ProbeContext> probe)
    {
        var probeContextMock = new Mock<ProbeContext>();
        var innerScopeMock = new Mock<ProbeContext>();
        probeContextMock.Setup(x => x.CreateScope(It.IsAny<string>())).Returns(innerScopeMock.Object);

        probe(probeContextMock.Object);

        probeContextMock.Verify(x => x.CreateScope("filters"), Times.Once);
    }
}
