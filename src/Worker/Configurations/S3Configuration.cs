using Amazon;
using Amazon.S3;

namespace Worker.Configurations;

/// <summary>
/// Configuração do client S3 para download de arquivos.
/// </summary>
public static class S3Configuration
{
    public static IServiceCollection AddS3(this IServiceCollection services, IConfiguration configuration)
    {
        var region = configuration["AWS:Region"] ?? "us-east-1";

        services.AddSingleton<IAmazonS3>(_ => new AmazonS3Client(RegionEndpoint.GetBySystemName(region)));

        return services;
    }
}
