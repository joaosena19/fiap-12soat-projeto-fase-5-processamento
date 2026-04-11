namespace Tests.Helpers.MockExtensions;

public static class ProcessamentoDiagramaMessagePublisherMockExtensions
{
    public static void AoPublicarNaoFazNada(this Mock<IProcessamentoDiagramaMessagePublisher> mock)
    {
        mock.Setup(x => x.PublicarProcessamentoIniciadoAsync(It.IsAny<ProcessamentoDiagramaAggregate>(), It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);
        mock.Setup(x => x.PublicarDiagramaAnalisadoAsync(It.IsAny<ProcessamentoDiagramaAggregate>())).Returns(Task.CompletedTask);
        mock.Setup(x => x.PublicarProcessamentoErroAsync(It.IsAny<ProcessamentoDiagramaAggregate>(), It.IsAny<string>())).Returns(Task.CompletedTask);
    }

    public static void DeveTerPublicadoProcessamentoIniciado(this Mock<IProcessamentoDiagramaMessagePublisher> mock)
    {
        mock.Verify(x => x.PublicarProcessamentoIniciadoAsync(It.IsAny<ProcessamentoDiagramaAggregate>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
    }

    public static void NaoDeveTerPublicadoProcessamentoIniciado(this Mock<IProcessamentoDiagramaMessagePublisher> mock)
    {
        mock.Verify(x => x.PublicarProcessamentoIniciadoAsync(It.IsAny<ProcessamentoDiagramaAggregate>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    public static void DeveTerPublicadoDiagramaAnalisado(this Mock<IProcessamentoDiagramaMessagePublisher> mock)
    {
        mock.Verify(x => x.PublicarDiagramaAnalisadoAsync(It.IsAny<ProcessamentoDiagramaAggregate>()), Times.Once);
    }

    public static void NaoDeveTerPublicadoDiagramaAnalisado(this Mock<IProcessamentoDiagramaMessagePublisher> mock)
    {
        mock.Verify(x => x.PublicarDiagramaAnalisadoAsync(It.IsAny<ProcessamentoDiagramaAggregate>()), Times.Never);
    }

    public static void DeveTerPublicadoProcessamentoErro(this Mock<IProcessamentoDiagramaMessagePublisher> mock)
    {
        mock.Verify(x => x.PublicarProcessamentoErroAsync(It.IsAny<ProcessamentoDiagramaAggregate>(), It.IsAny<string>()), Times.AtLeastOnce);
    }

    public static void DeveTerPublicadoProcessamentoErroComMensagem(this Mock<IProcessamentoDiagramaMessagePublisher> mock, string mensagem)
    {
        mock.Verify(x => x.PublicarProcessamentoErroAsync(It.IsAny<ProcessamentoDiagramaAggregate>(), mensagem), Times.Once);
    }

    public static void NaoDeveTerPublicadoProcessamentoErro(this Mock<IProcessamentoDiagramaMessagePublisher> mock)
    {
        mock.Verify(x => x.PublicarProcessamentoErroAsync(It.IsAny<ProcessamentoDiagramaAggregate>(), It.IsAny<string>()), Times.Never);
    }
}
