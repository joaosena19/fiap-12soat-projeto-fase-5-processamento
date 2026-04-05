using Application.Contracts.Messaging.Dtos;

namespace Tests.Application.Contracts.Messaging.Dtos;

public class UploadDiagramaConcluidoDtoTests
{
    [Fact(DisplayName = "Deve acessar todas as propriedades do DTO sem erro")]
    [Trait("Application", "UploadDiagramaConcluidoDto")]
    public void Propriedades_DevemSerAcessiveis_QuandoInstanciado()
    {
        // Arrange
        var analiseDiagramaId = Guid.NewGuid();
        var dataCriacao = DateTimeOffset.UtcNow;

        var dto = new UploadDiagramaConcluidoDto
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

        // Act & Assert
        dto.CorrelationId.ShouldBe("correlation-123");
        dto.AnaliseDiagramaId.ShouldBe(analiseDiagramaId);
        dto.NomeOriginal.ShouldBe("diagrama.png");
        dto.Extensao.ShouldBe(".png");
        dto.Tamanho.ShouldBe(1024L);
        dto.Hash.ShouldBe("abc123");
        dto.NomeFisico.ShouldBe("guid-fisico.png");
        dto.LocalizacaoUrl.ShouldBe("s3://bucket/diagramas/guid-fisico.png");
        dto.DataCriacao.ShouldBe(dataCriacao);
    }

    [Fact(DisplayName = "Deve ter valores padrão quando criado sem inicialização")]
    [Trait("Application", "UploadDiagramaConcluidoDto")]
    public void Construtor_DeveUsarValoresPadrao_QuandoCriadoSemInicializacao()
    {
        // Act
        var dto = new UploadDiagramaConcluidoDto();

        // Assert
        dto.CorrelationId.ShouldBe(string.Empty);
        dto.NomeOriginal.ShouldBe(string.Empty);
        dto.Extensao.ShouldBe(string.Empty);
        dto.Hash.ShouldBe(string.Empty);
        dto.NomeFisico.ShouldBe(string.Empty);
        dto.LocalizacaoUrl.ShouldBe(string.Empty);
        dto.AnaliseDiagramaId.ShouldBe(Guid.Empty);
        dto.Tamanho.ShouldBe(0L);
    }
}
