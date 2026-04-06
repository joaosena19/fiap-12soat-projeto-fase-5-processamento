using Application.Contracts.Messaging.Dtos;
using System.Text.Json;

namespace Tests.Application.Contracts.Messaging.Dtos;

public class UploadDiagramaConcluidoDtoTests
{
    [Fact(DisplayName = "Deve manter dados no roundtrip JSON do DTO")]
    [Trait("Application", "UploadDiagramaConcluidoDto")]
    public void RoundtripJson_DeveManterDados_QuandoSerializarEDeserializar()
    {
        // Arrange
        var analiseDiagramaId = Guid.NewGuid();
        var dataCriacao = DateTimeOffset.UtcNow;
        var original = new UploadDiagramaConcluidoDto
        {
            CorrelationId = "correlation-123",
            AnaliseDiagramaId = analiseDiagramaId,
            NomeOriginal = "diagrama.png",
            Extensao = ".png",
            Tamanho = 1024,
            Hash = "abc123",
            NomeFisico = "guid-fisico.png",
            LocalizacaoUrl = "s3://bucket/diagramas/guid-fisico.png",
            DataCriacao = dataCriacao
        };

        // Act
        var json = JsonSerializer.Serialize(original);
        var dto = JsonSerializer.Deserialize<UploadDiagramaConcluidoDto>(json);

        // Assert
        dto.ShouldNotBeNull();
        dto.DeveConterTodosOsDados("correlation-123", analiseDiagramaId, "diagrama.png", ".png", 1024L, "abc123", "guid-fisico.png", "s3://bucket/diagramas/guid-fisico.png", dataCriacao);
    }

    [Fact(DisplayName = "Deve ter valores padrão quando criado sem inicialização")]
    [Trait("Application", "UploadDiagramaConcluidoDto")]
    public void Construtor_DeveUsarValoresPadrao_QuandoCriadoSemInicializacao()
    {
        // Act
        var dto = new UploadDiagramaConcluidoDto();

        // Assert
        dto.DeveConterValoresPadrao();
    }
}
