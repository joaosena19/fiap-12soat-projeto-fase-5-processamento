using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Database;

/// <summary>
/// Contexto de banco de dados do serviço de Processamento.
/// </summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public DbSet<Domain.ProcessamentoDiagrama.Aggregates.ProcessamentoDiagrama> ProcessamentoDiagramas { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
