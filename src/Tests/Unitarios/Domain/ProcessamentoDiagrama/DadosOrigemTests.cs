using Domain.ProcessamentoDiagrama.Entities;

namespace Tests.Domain.ProcessamentoDiagrama;

public class DadosOrigemTests
{
    [Fact(DisplayName = "Deve criar dados de origem quando todos os valores sao validos")]
    [Trait("Entity", "DadosOrigem")]
    public void Criar_DeveCriar_QuandoValoresValidos()
    {
        // Arrange
        var localizacaoUrl = "s3://bucket/diagramas/arquivo-123.png";
        var nomeFisico = "arquivo-123.png";
        var nomeOriginal = "diagrama-arquitetura.png";
        var extensao = "png";

        // Act
        var dadosOrigem = DadosOrigem.Criar(localizacaoUrl, nomeFisico, nomeOriginal, extensao);

        // Assert
        dadosOrigem.LocalizacaoUrl.Valor.ShouldBe(localizacaoUrl);
        dadosOrigem.NomeFisico.Valor.ShouldBe(nomeFisico);
        dadosOrigem.NomeOriginal.Valor.ShouldBe(nomeOriginal);
        dadosOrigem.Extensao.Valor.ShouldBe(extensao);
    }

    [Fact(DisplayName = "Deve aplicar trim em todos os valores")]
    [Trait("Entity", "DadosOrigem")]
    public void Criar_DeveAplicarTrim_QuandoValoresComEspacos()
    {
        // Arrange & Act
        var dadosOrigem = DadosOrigem.Criar("  s3://bucket/arquivo.png  ", "  arquivo.png  ", "  original.png  ", "  png  ");

        // Assert
        dadosOrigem.LocalizacaoUrl.Valor.ShouldBe("s3://bucket/arquivo.png");
        dadosOrigem.NomeFisico.Valor.ShouldBe("arquivo.png");
        dadosOrigem.NomeOriginal.Valor.ShouldBe("original.png");
        dadosOrigem.Extensao.Valor.ShouldBe("png");
    }

    [Fact(DisplayName = "Deve lancar excecao quando localizacao url for vazia")]
    [Trait("Entity", "DadosOrigem")]
    public void Criar_DeveLancarExcecao_QuandoLocalizacaoUrlVazia()
    {
        // Arrange
        Action acao = () => DadosOrigem.Criar("", "arquivo.png", "original.png", "png");

        // Act & Assert
        acao.DeveLancarExcecaoDeValidacao("Localização URL não pode ser vazia");
    }

    [Fact(DisplayName = "Deve lancar excecao quando nome fisico for vazio")]
    [Trait("Entity", "DadosOrigem")]
    public void Criar_DeveLancarExcecao_QuandoNomeFisicoVazio()
    {
        // Arrange
        Action acao = () => DadosOrigem.Criar("s3://bucket/arquivo.png", "", "original.png", "png");

        // Act & Assert
        acao.DeveLancarExcecaoDeValidacao("Nome físico não pode ser vazio");
    }

    [Fact(DisplayName = "Deve lancar excecao quando nome original for vazio")]
    [Trait("Entity", "DadosOrigem")]
    public void Criar_DeveLancarExcecao_QuandoNomeOriginalVazio()
    {
        // Arrange
        Action acao = () => DadosOrigem.Criar("s3://bucket/arquivo.png", "arquivo.png", "", "png");

        // Act & Assert
        acao.DeveLancarExcecaoDeValidacao("Nome original não pode ser vazio");
    }

    [Fact(DisplayName = "Deve lancar excecao quando extensao for vazia")]
    [Trait("Entity", "DadosOrigem")]
    public void Criar_DeveLancarExcecao_QuandoExtensaoVazia()
    {
        // Arrange
        Action acao = () => DadosOrigem.Criar("s3://bucket/arquivo.png", "arquivo.png", "original.png", "");

        // Act & Assert
        acao.DeveLancarExcecaoDeValidacao("Extensão do arquivo não pode ser vazia");
    }
}
