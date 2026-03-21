using Application.Contracts.Gateways;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

/// <summary>
/// Repositório para operações de persistência de ProcessamentoDiagrama.
/// </summary>
public class ProcessamentoDiagramaRepository : IProcessamentoDiagramaGateway
{
    private readonly AppDbContext _context;

    public ProcessamentoDiagramaRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Domain.ProcessamentoDiagrama.Aggregates.ProcessamentoDiagrama> SalvarAsync(Domain.ProcessamentoDiagrama.Aggregates.ProcessamentoDiagrama processamentoDiagrama)
    {
        var existente = await _context.ProcessamentoDiagramas.FindAsync(processamentoDiagrama.Id);

        if (existente == null)
            await _context.ProcessamentoDiagramas.AddAsync(processamentoDiagrama);

        await _context.SaveChangesAsync();
        return processamentoDiagrama;
    }

    public async Task<Domain.ProcessamentoDiagrama.Aggregates.ProcessamentoDiagrama?> ObterPorAnaliseDiagramaIdAsync(Guid analiseDiagramaId)
    {
        return await _context.ProcessamentoDiagramas
            .FirstOrDefaultAsync(p => p.AnaliseDiagramaId == analiseDiagramaId);
    }
}
