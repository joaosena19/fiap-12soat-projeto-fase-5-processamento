using Application.ProcessamentoDiagrama.UseCases;

namespace Tests.Helpers.Fixtures;

public class ProcessarDiagramaUseCaseTestFixture
{
    public Mock<IProcessamentoDiagramaGateway> GatewayMock { get; }
    public Mock<IDiagramaAnaliseService> LlmServiceMock { get; }
    public Mock<IProcessamentoDiagramaMessagePublisher> MessagePublisherMock { get; }
    public Mock<IMetricsService> MetricsMock { get; }
    public Mock<IAppLogger> LoggerMock { get; }
    public ProcessarDiagramaUseCase UseCase { get; }

    public ProcessarDiagramaUseCaseTestFixture()
    {
        GatewayMock = new Mock<IProcessamentoDiagramaGateway>();
        LlmServiceMock = new Mock<IDiagramaAnaliseService>();
        MessagePublisherMock = new Mock<IProcessamentoDiagramaMessagePublisher>();
        MetricsMock = new Mock<IMetricsService>();
        LoggerMock = MockLogger.Criar();
        UseCase = new ProcessarDiagramaUseCase();
    }

    public async Task ExecutarAsync(ProcessarDiagramaDto? processarDiagramaDto = null)
    {
        processarDiagramaDto ??= new ProcessarDiagramaDtoBuilder().Build();

        await UseCase.ExecutarAsync(processarDiagramaDto, GatewayMock.Object, LlmServiceMock.Object, MessagePublisherMock.Object, MetricsMock.Object, LoggerMock.Object);
    }
}
