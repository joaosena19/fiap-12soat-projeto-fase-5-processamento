using Application.Contracts.Messaging;
using Application.Contracts.Messaging.Dtos;
using Infrastructure.Messaging;
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
                var region = configuration["AWS:Region"] ?? "us-east-1";

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

                cfg.Message<UploadDiagramaConcluidoDto>(m => m.SetEntityName("fase5-upload-diagrama-concluido"));
                cfg.Message<ProcessamentoDiagramaIniciadoDto>(m => m.SetEntityName("fase5-processamento-diagrama-iniciado"));
                cfg.Message<ProcessamentoDiagramaAnalisadoDto>(m => m.SetEntityName("fase5-processamento-diagrama-analisado"));
                cfg.Message<ProcessamentoDiagramaErroDto>(m => m.SetEntityName("fase5-processamento-diagrama-erro"));

                cfg.ReceiveEndpoint("fase5-upload-diagrama-concluido", e =>
                {
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