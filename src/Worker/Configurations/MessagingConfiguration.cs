using Application.Contracts.Messaging;
using Application.Contracts.Messaging.Dtos;
using Infrastructure.Messaging;
using Infrastructure.Messaging.Consumers;
using Infrastructure.Messaging.Filters;
using Infrastructure.Messaging.Publishers;
using MassTransit;
using System.Text.Json.Serialization;

namespace Worker.Configurations;

public static class MessagingConfiguration
{
    public static IServiceCollection AddMessaging(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMassTransit(x =>
        {
            x.AddConsumer<UploadDiagramaConcluidoConsumer>();
            x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter(false));

            x.UsingAmazonSqs((context, cfg) =>
            {
                var region = configuration["AWS:Region"] ?? throw new InvalidOperationException("Configuração AWS:Region não encontrada");

                cfg.Host(region, h =>
                {
                    var accessKey = configuration["AWS:AccessKeyId"];
                    var secretKey = configuration["AWS:SecretAccessKey"];

                    if (!string.IsNullOrEmpty(accessKey) && !string.IsNullOrEmpty(secretKey))
                    {
                        h.AccessKey(accessKey);
                        h.SecretKey(secretKey);
                    }
                });

                var topicoUploadConcluido = configuration["Mensageria:Topicos:UploadDiagramaConcluido"]!;
                var filaUploadConcluido = configuration["Mensageria:Filas:UploadDiagramaConcluido"] ?? topicoUploadConcluido;

                cfg.Message<UploadDiagramaConcluidoDto>(m => m.SetEntityName(topicoUploadConcluido));
                cfg.Message<ProcessamentoDiagramaIniciadoDto>(m => m.SetEntityName(configuration["Mensageria:Topicos:ProcessamentoDiagramaIniciado"]!));
                cfg.Message<ProcessamentoDiagramaAnalisadoDto>(m => m.SetEntityName(configuration["Mensageria:Topicos:ProcessamentoDiagramaAnalisado"]!));
                cfg.Message<ProcessamentoDiagramaErroDto>(m => m.SetEntityName(configuration["Mensageria:Topicos:ProcessamentoDiagramaErro"]!));

                cfg.ReceiveEndpoint(filaUploadConcluido, e =>
                {
                    e.ConcurrentMessageLimit = 1;
                    e.PrefetchCount = 1;
                    e.ConfigureConsumer<UploadDiagramaConcluidoConsumer>(context);
                });

                cfg.UseSendFilter(typeof(SendCorrelationIdFilter<>), context);
                cfg.UsePublishFilter(typeof(PublishCorrelationIdFilter<>), context);
                cfg.UseConsumeFilter(typeof(ConsumeCorrelationIdFilter<>), context);

                cfg.ConfigureJsonSerializerOptions(options =>
                {
                    options.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
                    options.Converters.Add(new JsonStringEnumConverter());
                    return options;
                });
            });
        });

        services.AddScoped<IProcessamentoDiagramaMessagePublisher, ProcessamentoDiagramaMessagePublisher>();

        return services;
    }
}