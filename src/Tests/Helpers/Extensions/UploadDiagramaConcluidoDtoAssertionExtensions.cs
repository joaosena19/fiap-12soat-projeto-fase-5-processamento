using Application.Contracts.Messaging.Dtos;

namespace Tests.Helpers.Extensions;

public static class UploadDiagramaConcluidoDtoAssertionExtensions
{
    public static void DeveConterTodosOsDados(this UploadDiagramaConcluidoDto dto, string correlationId, Guid analiseDiagramaId, string nomeOriginal, string extensao, long tamanho, string hash, string nomeFisico, string localizacaoUrl, DateTimeOffset dataCriacao)
    {
        dto.CorrelationId.ShouldBe(correlationId);
        dto.AnaliseDiagramaId.ShouldBe(analiseDiagramaId);
        dto.NomeOriginal.ShouldBe(nomeOriginal);
        dto.Extensao.ShouldBe(extensao);
        dto.Tamanho.ShouldBe(tamanho);
        dto.Hash.ShouldBe(hash);
        dto.NomeFisico.ShouldBe(nomeFisico);
        dto.LocalizacaoUrl.ShouldBe(localizacaoUrl);
        dto.DataCriacao.ShouldBe(dataCriacao);
    }

    public static void DeveConterValoresPadrao(this UploadDiagramaConcluidoDto dto)
    {
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
