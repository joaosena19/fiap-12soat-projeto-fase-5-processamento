namespace Tests.Helpers.Builders;

public class ProcessarDiagramaDtoBuilder
{
    private Guid _analiseDiagramaId = Guid.NewGuid();
    private string _nomeOriginal = "diagrama-arquitetura.png";
    private string _extensao = "png";
    private string _nomeFisico = "diagrama-123.png";
    private string _localizacaoUrl = "s3://bucket/diagrama-123.png";

    public ProcessarDiagramaDtoBuilder ComAnaliseDiagramaId(Guid analiseDiagramaId)
    {
        _analiseDiagramaId = analiseDiagramaId;
        return this;
    }

    public ProcessarDiagramaDtoBuilder ComArquivo(string nomeOriginal, string extensao, string nomeFisico, string localizacaoUrl)
    {
        _nomeOriginal = nomeOriginal;
        _extensao = extensao;
        _nomeFisico = nomeFisico;
        _localizacaoUrl = localizacaoUrl;
        return this;
    }

    public ProcessarDiagramaDto Build()
    {
        return new ProcessarDiagramaDto
        {
            AnaliseDiagramaId = _analiseDiagramaId,
            NomeOriginal = _nomeOriginal,
            Extensao = _extensao,
            NomeFisico = _nomeFisico,
            LocalizacaoUrl = _localizacaoUrl
        };
    }
}
