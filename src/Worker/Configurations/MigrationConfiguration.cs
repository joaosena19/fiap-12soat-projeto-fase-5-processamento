using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Worker.Configurations;

public static class MigrationConfiguration
{
    public static void AplicarMigracoes(this IHost host)
    {
        using var scope = host.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        dbContext.Database.Migrate();
    }
}
