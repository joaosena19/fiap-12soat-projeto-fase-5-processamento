using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Tests.Helpers.Fixtures;

public class AppDbContextTestFixture : IDisposable
{
    public AppDbContext Context { get; }

    public AppDbContextTestFixture()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase($"processamento-tests-{Guid.NewGuid()}")
            .Options;

        Context = new AppDbContext(options);
    }

    public void Dispose()
    {
        Context.Dispose();
    }
}
