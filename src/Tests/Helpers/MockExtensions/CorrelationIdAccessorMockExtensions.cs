namespace Tests.Helpers.MockExtensions;

public static class CorrelationIdAccessorMockExtensions
{
    public static void AoObterCorrelationId(this Mock<ICorrelationIdAccessor> mock, string? correlationId = null)
    {
        mock.Setup(x => x.GetCorrelationId()).Returns(correlationId ?? Guid.NewGuid().ToString());
    }
}
