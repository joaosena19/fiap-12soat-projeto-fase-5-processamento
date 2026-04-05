using Domain.ProcessamentoDiagrama.Aggregates;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Tests.Infrastructure.Database.Configurations;

public class ProcessamentoDiagramaConfigurationTests
{
    [Fact(DisplayName = "Deve mapear entidade para tabela e índice esperados")]
    [Trait("Infrastructure", "ProcessamentoDiagramaConfiguration")]
    public void Modelo_DeveMapearTabelaEIndice_QuandoContextoInicializar()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase($"config-tests-{Guid.NewGuid()}")
            .Options;

        using var context = new AppDbContext(options);

        // Act
        var entityType = context.Model.FindEntityType(typeof(ProcessamentoDiagrama));

        // Assert
        entityType.ShouldNotBeNull();
        entityType.GetTableName().ShouldBe("processamento_diagramas");
        entityType.GetIndexes().Any(index => index.Properties.Any(property => property.Name == "AnaliseDiagramaId") && index.IsUnique).ShouldBeTrue();
    }

    [Fact(DisplayName = "Deve persistir coleções da análise quando processamento é concluído")]
    [Trait("Infrastructure", "ProcessamentoDiagramaConfiguration")]
    public async Task Persistencia_DeveManterColecoes_QuandoProcessamentoConcluido()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase($"config-tests-{Guid.NewGuid()}")
            .Options;

        var analiseDiagramaId = Guid.NewGuid();

        await using (var context = new AppDbContext(options))
        {
            var processamento = new ProcessamentoDiagramaBuilder()
                .ComAnaliseDiagramaId(analiseDiagramaId)
                .Concluido()
                .Build();

            await context.ProcessamentoDiagramas.AddAsync(processamento);
            await context.SaveChangesAsync();
        }

        await using var contextoLeitura = new AppDbContext(options);

        // Act
        var obtido = await contextoLeitura.ProcessamentoDiagramas.AsNoTracking().FirstOrDefaultAsync(x => x.AnaliseDiagramaId == analiseDiagramaId);

        // Assert
        obtido.ShouldNotBeNull();
        obtido.AnaliseResultado.ShouldNotBeNull();
        obtido.AnaliseResultado.ComponentesIdentificados.Count.ShouldBeGreaterThan(0);
        obtido.AnaliseResultado.RiscosArquiteturais.Count.ShouldBeGreaterThan(0);
        obtido.AnaliseResultado.RecomendacoesBasicas.Count.ShouldBeGreaterThan(0);
    }
}
