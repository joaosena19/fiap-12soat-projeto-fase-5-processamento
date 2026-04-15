using Domain.ProcessamentoDiagrama.ValueObjects;
using Shared.Attributes;

namespace Domain.ProcessamentoDiagrama.Entities;

[AggregateMember]
public class DadosOrigem
{
    public LocalizacaoUrl LocalizacaoUrl { get; private set; } = null!;
    public NomeFisico NomeFisico { get; private set; } = null!;
    public NomeOriginal NomeOriginal { get; private set; } = null!;
    public ExtensaoArquivo Extensao { get; private set; } = null!;

    private DadosOrigem() { }

    private DadosOrigem(LocalizacaoUrl localizacaoUrl, NomeFisico nomeFisico, NomeOriginal nomeOriginal, ExtensaoArquivo extensao)
    {
        LocalizacaoUrl = localizacaoUrl;
        NomeFisico = nomeFisico;
        NomeOriginal = nomeOriginal;
        Extensao = extensao;
    }

    public static DadosOrigem Criar(string localizacaoUrl, string nomeFisico, string nomeOriginal, string extensao)
    {
        return new DadosOrigem(
            new LocalizacaoUrl(localizacaoUrl),
            new NomeFisico(nomeFisico),
            new NomeOriginal(nomeOriginal),
            new ExtensaoArquivo(extensao));
    }
}
