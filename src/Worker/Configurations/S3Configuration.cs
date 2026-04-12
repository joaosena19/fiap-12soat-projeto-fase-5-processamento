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
        var regionEndpoint = RegionEndpoint.GetBySystemName(region);

        var accessKey = configuration["AWS:AccessKeyId"];
        var secretKey = configuration["AWS:SecretAccessKey"];

        if (!string.IsNullOrEmpty(accessKey) && !string.IsNullOrEmpty(secretKey))
            services.AddSingleton<IAmazonS3>(new AmazonS3Client(accessKey, secretKey, regionEndpoint));
        else
            services.AddSingleton<IAmazonS3>(new AmazonS3Client(regionEndpoint));

        services.AddScoped<IArquivoDiagramaDownloader, S3ArquivoDownloader>();

        return services;
    }
}
