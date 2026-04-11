using Application.Contracts.Messaging.Dtos;
using Application.Contracts.Monitoramento;
using Infrastructure.Messaging.Publishers;
using MassTransit;

namespace Tests.Helpers.Fixtures;

public class ProcessamentoDiagramaMessagePublisherTestFixture
{
    public Mock<IPublishEndpoint> PublishEndpointMock { get; }
    public Mock<ICorrelationIdAccessor> CorrelationIdAccessorMock { get; }
    public ILoggerFactory LoggerFactory { get; }
    public ProcessamentoDiagramaMessagePublisher Publisher { get; }

    public ProcessamentoDiagramaMessagePublisherTestFixture()
    {
        PublishEndpointMock = new Mock<IPublishEndpoint>();
        CorrelationIdAccessorMock = new Mock<ICorrelationIdAccessor>();
        LoggerFactory = new LoggerFactory();

        CorrelationIdAccessorMock.AoObterCorrelationId();

        PublishEndpointMock.AoPublicarNaoFazNada<ProcessamentoDiagramaIniciadoDto>();
        PublishEndpointMock.AoPublicarNaoFazNada<ProcessamentoDiagramaAnalisadoDto>();
        PublishEndpointMock.AoPublicarNaoFazNada<ProcessamentoDiagramaErroDto>();

        Publisher = new ProcessamentoDiagramaMessagePublisher(PublishEndpointMock.Object, CorrelationIdAccessorMock.Object, LoggerFactory);
    }
}
