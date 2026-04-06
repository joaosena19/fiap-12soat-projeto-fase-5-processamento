using Infrastructure.Repositories;
using Tests.Helpers.Fixtures;

namespace Tests.Infrastructure.Repositories;

public class ProcessamentoDiagramaRepositoryTests : IDisposable
{
    private readonly AppDbContextTestFixture _fixture;
    private readonly ProcessamentoDiagramaRepository _repository;

    public ProcessamentoDiagramaRepositoryTests()
    {
        _fixture = new AppDbContextTestFixture();
        _repository = new ProcessamentoDiagramaRepository(_fixture.Context);
    }

    [Fact(DisplayName = "Deve salvar processamento quando registro não existe")]
    [Trait("Infrastructure", "ProcessamentoDiagramaRepository")]
    public async Task SalvarAsync_DevePersistirRegistro_QuandoNaoExiste()
    {
        // Arrange
        var processamento = new ProcessamentoDiagramaBuilder().Build();

        // Act
        await _repository.SalvarAsync(processamento);
        var obtido = await _repository.ObterPorAnaliseDiagramaIdAsync(processamento.AnaliseDiagramaId);

        // Assert
        obtido.ShouldNotBeNull();
        obtido.Id.ShouldBe(processamento.Id);
    }

    [Fact(DisplayName = "Deve atualizar processamento quando registro já existe")]
    [Trait("Infrastructure", "ProcessamentoDiagramaRepository")]
    public async Task SalvarAsync_DeveAtualizarRegistro_QuandoJaExiste()
    {
        // Arrange
        var processamento = new ProcessamentoDiagramaBuilder().Build();
        await _repository.SalvarAsync(processamento);

        processamento.IniciarProcessamento();

        // Act
        await _repository.SalvarAsync(processamento);
        var obtido = await _repository.ObterPorAnaliseDiagramaIdAsync(processamento.AnaliseDiagramaId);

        // Assert
        obtido.ShouldNotBeNull();
        obtido.StatusProcessamento.Valor.ShouldBe(StatusProcessamentoEnum.EmProcessamento);
    }

    [Fact(DisplayName = "Deve retornar nulo quando análise não existe")]
    [Trait("Infrastructure", "ProcessamentoDiagramaRepository")]
    public async Task ObterPorAnaliseDiagramaIdAsync_DeveRetornarNulo_QuandoNaoEncontrar()
    {
        // Act
        var obtido = await _repository.ObterPorAnaliseDiagramaIdAsync(Guid.NewGuid());

        // Assert
        obtido.ShouldBeNull();
    }

    public void Dispose()
    {
        _fixture.Dispose();
    }
}
