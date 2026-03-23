using Amazon;
using Amazon.S3;
using Infrastructure.Armazenamento;

namespace Worker.Configurations;

/// <summary>
/// Configuração do client S3 para download de arquivos.
/// </summary>
public static class S3Configuration
{
    public static IServiceCollection AddS3(this IServiceCollection services, IConfiguration configuration)
    {
        var region = configuration["AWS:Region"] ?? throw new InvalidOperationException("Configuração AWS:Region não encontrada");

        services.AddSingleton<IAmazonS3>(_ => new AmazonS3Client(RegionEndpoint.GetBySystemName(region)));
        services.AddScoped<IArquivoDiagramaDownloader, S3ArquivoDownloader>();

        return services;
    }
}
