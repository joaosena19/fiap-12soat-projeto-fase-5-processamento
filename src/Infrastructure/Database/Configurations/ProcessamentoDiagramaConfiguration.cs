using Domain.ProcessamentoDiagrama.Aggregates;
using Domain.ProcessamentoDiagrama.Enums;
using Domain.ProcessamentoDiagrama.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Text.Json;

namespace Infrastructure.Database.Configurations;

public class ProcessamentoDiagramaConfiguration : IEntityTypeConfiguration<ProcessamentoDiagrama>
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    public void Configure(EntityTypeBuilder<ProcessamentoDiagrama> builder)
    {
        builder.ToTable("processamento_diagramas");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).HasColumnName("id");
        builder.Property(p => p.AnaliseDiagramaId).HasColumnName("analise_diagrama_id").IsRequired();

        builder.OwnsOne(p => p.StatusProcessamento, statusProcessamento =>
        {
            statusProcessamento.Property(v => v.Valor)
                .HasColumnName("status_processamento")
                .IsRequired()
                .HasConversion(
                    v => v.ToString().ToLower(),
                    v => Enum.Parse<StatusProcessamentoEnum>(v, true));
        });

        builder.OwnsOne(p => p.TentativasProcessamento, tentativas =>
        {
            tentativas.Property(v => v.Valor).HasColumnName("tentativas_processamento").IsRequired();
        });

        builder.OwnsOne(p => p.HistoricoTemporal, historico =>
        {
            historico.Property(h => h.DataCriacao).HasColumnName("data_criacao").IsRequired();
            historico.Property(h => h.DataInicioProcessamento).HasColumnName("data_inicio_processamento");
            historico.Property(h => h.DataConclusaoProcessamento).HasColumnName("data_conclusao_processamento");
        });

        builder.OwnsOne(p => p.AnaliseResultado, analiseResultado =>
        {
            analiseResultado.OwnsOne(a => a.DescricaoAnalise, descricao =>
            {
                descricao.Property(d => d.Valor).HasColumnName("descricao_analise").HasMaxLength(10000);
            });

            analiseResultado.Property(a => a.ComponentesIdentificados)
                .HasColumnName("componentes_identificados")
                .HasConversion(
                    v => JsonSerializer.Serialize(v.Select(item => item.Valor).ToList(), JsonOptions),
                    v => string.IsNullOrEmpty(v) ? new List<ComponenteIdentificado>() : (JsonSerializer.Deserialize<List<string>>(v, JsonOptions) ?? new List<string>()).Select(item => new ComponenteIdentificado(item)).ToList());

            analiseResultado.Property(a => a.RiscosArquiteturais)
                .HasColumnName("riscos_arquiteturais")
                .HasConversion(
                    v => JsonSerializer.Serialize(v.Select(item => item.Valor).ToList(), JsonOptions),
                    v => string.IsNullOrEmpty(v) ? new List<RiscoArquitetural>() : (JsonSerializer.Deserialize<List<string>>(v, JsonOptions) ?? new List<string>()).Select(item => new RiscoArquitetural(item)).ToList());

            analiseResultado.Property(a => a.RecomendacoesBasicas)
                .HasColumnName("recomendacoes_basicas")
                .HasConversion(
                    v => JsonSerializer.Serialize(v.Select(item => item.Valor).ToList(), JsonOptions),
                    v => string.IsNullOrEmpty(v) ? new List<RecomendacaoBasica>() : (JsonSerializer.Deserialize<List<string>>(v, JsonOptions) ?? new List<string>()).Select(item => new RecomendacaoBasica(item)).ToList());
        });

        builder.HasIndex(p => p.AnaliseDiagramaId).IsUnique();

        builder.OwnsOne(p => p.DadosOrigem, dadosOrigem =>
        {
            dadosOrigem.Property(d => d.LocalizacaoUrl)
                .HasConversion(v => v.Valor, v => new LocalizacaoUrl(v))
                .HasColumnName("localizacao_url")
                .IsRequired()
                .HasMaxLength(500);

            dadosOrigem.Property(d => d.NomeFisico)
                .HasConversion(v => v.Valor, v => new NomeFisico(v))
                .HasColumnName("nome_fisico")
                .HasMaxLength(200);

            dadosOrigem.Property(d => d.NomeOriginal)
                .HasConversion(v => v.Valor, v => new NomeOriginal(v))
                .HasColumnName("nome_original")
                .HasMaxLength(500);

            dadosOrigem.Property(d => d.Extensao)
                .HasConversion(v => v.Valor, v => new ExtensaoArquivo(v))
                .HasColumnName("extensao")
                .HasMaxLength(20);
        });
    }
}
