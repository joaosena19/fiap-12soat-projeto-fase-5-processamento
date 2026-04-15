using Application.Contracts.Messaging.Dtos;

namespace Tests.Helpers.Builders;

public class UploadDiagramaConcluidoDtoBuilder
{
    private string _correlationId = Guid.NewGuid().ToString();
    private Guid _analiseDiagramaId = Guid.NewGuid();
    private string _nomeOriginal = "diagrama.png";
    private string _extensao = ".png";
    private long _tamanho = 1024;
    private string _hash = "abc123hash";
    private string _nomeFisico = "arquivo-fisico.png";
    private string _localizacaoUrl = "https://bucket/arquivo-fisico.png";
    private DateTimeOffset _dataCriacao = DateTimeOffset.UtcNow;

    public UploadDiagramaConcluidoDtoBuilder ComCorrelationId(string valor) { _correlationId = valor; return this; }
    public UploadDiagramaConcluidoDtoBuilder ComAnaliseDiagramaId(Guid valor) { _analiseDiagramaId = valor; return this; }
    public UploadDiagramaConcluidoDtoBuilder ComNomeOriginal(string valor) { _nomeOriginal = valor; return this; }
    public UploadDiagramaConcluidoDtoBuilder ComExtensao(string valor) { _extensao = valor; return this; }
    public UploadDiagramaConcluidoDtoBuilder ComTamanho(long valor) { _tamanho = valor; return this; }
    public UploadDiagramaConcluidoDtoBuilder ComHash(string valor) { _hash = valor; return this; }
    public UploadDiagramaConcluidoDtoBuilder ComNomeFisico(string valor) { _nomeFisico = valor; return this; }
    public UploadDiagramaConcluidoDtoBuilder ComLocalizacaoUrl(string valor) { _localizacaoUrl = valor; return this; }

    public UploadDiagramaConcluidoDtoBuilder SemLocalizacaoUrl() { _localizacaoUrl = string.Empty; return this; }

    public UploadDiagramaConcluidoDto Build() => new()
    {
        CorrelationId = _correlationId,
        AnaliseDiagramaId = _analiseDiagramaId,
        NomeOriginal = _nomeOriginal,
        Extensao = _extensao,
        Tamanho = _tamanho,
        Hash = _hash,
        NomeFisico = _nomeFisico,
        LocalizacaoUrl = _localizacaoUrl,
        DataCriacao = _dataCriacao
    };
}
