using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Worker.Configurations;

public static class DatabaseConfiguration
{
    public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        var dbConfig = configuration.GetSection("DatabaseConnection");
        var dbHost = dbConfig["Host"];
        var dbPort = dbConfig["Port"];
        var dbName = dbConfig["DatabaseName"];
        var dbUser = dbConfig["User"];
        var dbPassword = dbConfig["Password"];

        if (string.IsNullOrEmpty(dbHost) || string.IsNullOrEmpty(dbPort) || string.IsNullOrEmpty(dbName) || string.IsNullOrEmpty(dbUser) || string.IsNullOrEmpty(dbPassword))
        {
            throw new InvalidOperationException(
                $"Configuração de banco de dados incompleta. Host: {dbHost}, Port: {dbPort}, Database: {dbName}, User: {dbUser}, Password: {(string.IsNullOrEmpty(dbPassword) ? "VAZIO" : "DEFINIDO")}");
        }

        var connectionString = $"Host={dbHost};Port={dbPort};Database={dbName};Username={dbUser};Password={dbPassword}";

        services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));

        return services;
    }
}