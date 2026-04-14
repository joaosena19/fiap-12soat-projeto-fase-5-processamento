namespace Tests.Helpers.MockExtensions;

public static class ProcessamentoDiagramaGatewayMockExtensions
{
    public static ObterPorAnaliseDiagramaIdSetup AoObterPorAnaliseDiagramaId(this Mock<IProcessamentoDiagramaGateway> mock, Guid? analiseDiagramaId = null) => new(mock, analiseDiagramaId);

    public static ObterPorAnaliseDiagramaIdSequencialSetup AoObterPorAnaliseDiagramaIdSequencial(this Mock<IProcessamentoDiagramaGateway> mock, Guid analiseDiagramaId) => new(mock, analiseDiagramaId);

    public static SalvarSetup AoSalvar(this Mock<IProcessamentoDiagramaGateway> mock) => new(mock);

    public static void DeveTerSalvo(this Mock<IProcessamentoDiagramaGateway> mock)
    {
        mock.Verify(x => x.SalvarAsync(It.IsAny<ProcessamentoDiagramaAggregate>()), Times.AtLeastOnce);
    }

    public static void DeveTerSalvo(this Mock<IProcessamentoDiagramaGateway> mock, int vezes)
    {
        mock.Verify(x => x.SalvarAsync(It.IsAny<ProcessamentoDiagramaAggregate>()), Times.Exactly(vezes));
    }

    public static void NaoDeveTerSalvo(this Mock<IProcessamentoDiagramaGateway> mock)
    {
        mock.Verify(x => x.SalvarAsync(It.IsAny<ProcessamentoDiagramaAggregate>()), Times.Never);
    }

    public class ObterPorAnaliseDiagramaIdSetup
    {
        private readonly Mock<IProcessamentoDiagramaGateway> _mock;
        private readonly Guid? _analiseDiagramaId;

        public ObterPorAnaliseDiagramaIdSetup(Mock<IProcessamentoDiagramaGateway> mock, Guid? analiseDiagramaId)
        {
            _mock = mock;
            _analiseDiagramaId = analiseDiagramaId;
        }

        public void Retorna(ProcessamentoDiagramaAggregate processamento)
        {
            if (_analiseDiagramaId.HasValue)
                _mock.Setup(x => x.ObterPorAnaliseDiagramaIdAsync(_analiseDiagramaId.Value)).ReturnsAsync(processamento);
            else
                _mock.Setup(x => x.ObterPorAnaliseDiagramaIdAsync(It.IsAny<Guid>())).ReturnsAsync(processamento);
        }

        public void NaoRetornaNada()
        {
            if (_analiseDiagramaId.HasValue)
                _mock.Setup(x => x.ObterPorAnaliseDiagramaIdAsync(_analiseDiagramaId.Value)).ReturnsAsync((ProcessamentoDiagramaAggregate?)null);
            else
                _mock.Setup(x => x.ObterPorAnaliseDiagramaIdAsync(It.IsAny<Guid>())).ReturnsAsync((ProcessamentoDiagramaAggregate?)null);
        }
    }

    public class ObterPorAnaliseDiagramaIdSequencialSetup
    {
        private readonly Mock<IProcessamentoDiagramaGateway> _mock;
        private readonly Guid _analiseDiagramaId;

        public ObterPorAnaliseDiagramaIdSequencialSetup(Mock<IProcessamentoDiagramaGateway> mock, Guid analiseDiagramaId)
        {
            _mock = mock;
            _analiseDiagramaId = analiseDiagramaId;
        }

        public void RetornaPrimeiro(ProcessamentoDiagramaAggregate? primeiro, ProcessamentoDiagramaAggregate? segundo)
        {
            _mock.SetupSequence(x => x.ObterPorAnaliseDiagramaIdAsync(_analiseDiagramaId))
                .ReturnsAsync(primeiro)
                .ReturnsAsync(segundo);
        }

        public void RetornaPrimeiro(ProcessamentoDiagramaAggregate? primeiro, Func<ProcessamentoDiagramaAggregate> segundoFactory)
        {
            var chamadas = 0;
            _mock.Setup(x => x.ObterPorAnaliseDiagramaIdAsync(_analiseDiagramaId))
                .ReturnsAsync(() => chamadas++ == 0 ? primeiro : segundoFactory());
        }
    }

    public class SalvarSetup
    {
        private readonly Mock<IProcessamentoDiagramaGateway> _mock;

        public SalvarSetup(Mock<IProcessamentoDiagramaGateway> mock)
        {
            _mock = mock;
        }

        public void ComCallback(Action<ProcessamentoDiagramaAggregate> callback)
        {
            _mock.Setup(x => x.SalvarAsync(It.IsAny<ProcessamentoDiagramaAggregate>()))
                .Callback<ProcessamentoDiagramaAggregate>(callback)
                .ReturnsAsync((ProcessamentoDiagramaAggregate p) => p);
        }
    }
}
