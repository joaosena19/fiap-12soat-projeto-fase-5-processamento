using Infrastructure.Handlers;
using Application.ProcessamentoDiagrama.UseCases;

namespace Tests.Helpers.Fixtures;

public class ProcessamentoDiagramaHandlerTestFixture
{
    public Mock<IProcessamentoDiagramaGateway> GatewayMock { get; }
    public Mock<IDiagramaAnaliseService> LlmServiceMock { get; }
    public Mock<IProcessamentoDiagramaMessagePublisher> MessagePublisherMock { get; }
    public Mock<IMetricsService> MetricsMock { get; }
    public Mock<IAppLogger> LoggerMock { get; }
    public Mock<ILoggerFactory> LoggerFactoryMock { get; }
    public ProcessamentoDiagramaHandler Handler { get; }

    public ProcessamentoDiagramaHandlerTestFixture()
    {
        GatewayMock = new Mock<IProcessamentoDiagramaGateway>();
        LlmServiceMock = new Mock<IDiagramaAnaliseService>();
        MessagePublisherMock = new Mock<IProcessamentoDiagramaMessagePublisher>();
        MetricsMock = new Mock<IMetricsService>();
        LoggerMock = MockLogger.Criar();
        LoggerFactoryMock = LoggerFactoryMockExtensions.CriarLoggerFactoryMock();

        Handler = new ProcessamentoDiagramaHandler(LoggerFactoryMock.Object);
    }

    public async Task IniciarProcessamentoAsync(ProcessarDiagramaDto? processarDiagramaDto = null)
    {
        processarDiagramaDto ??= new ProcessarDiagramaDtoBuilder().Build();

        await Handler.IniciarProcessamentoAsync(processarDiagramaDto, GatewayMock.Object, LlmServiceMock.Object, MessagePublisherMock.Object, MetricsMock.Object, LoggerMock.Object);
    }
}
