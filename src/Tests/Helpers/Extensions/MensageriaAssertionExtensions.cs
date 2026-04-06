using Application.Contracts.Messaging.Dtos;

namespace Tests.Helpers.Extensions;

public static class MensageriaAssertionExtensions
{
    public static void DeveConterDadosInicializacao(this ProcessamentoDiagramaIniciadoDto mensagem, string correlationId, ProcessamentoDiagramaAggregate processamento, string nomeOriginal, string extensao)
    {
        mensagem.CorrelationId.ShouldBe(correlationId);
        mensagem.AnaliseDiagramaId.ShouldBe(processamento.AnaliseDiagramaId);
        mensagem.NomeOriginal.ShouldBe(nomeOriginal);
        mensagem.Extensao.ShouldBe(extensao);
        mensagem.DataInicio.ShouldBe(processamento.HistoricoTemporal.DataInicioProcessamento!.Value);
    }

    public static void DeveConterDadosConclusao(this ProcessamentoDiagramaAnalisadoDto mensagem, string correlationId, ProcessamentoDiagramaAggregate processamento)
    {
        mensagem.CorrelationId.ShouldBe(correlationId);
        mensagem.AnaliseDiagramaId.ShouldBe(processamento.AnaliseDiagramaId);
        mensagem.DescricaoAnalise.ShouldBe(processamento.AnaliseResultado!.DescricaoAnalise.Valor);
        mensagem.ComponentesIdentificados.ShouldBe(processamento.AnaliseResultado.ComponentesIdentificados.Select(item => item.Valor).ToList());
        mensagem.RiscosArquiteturais.ShouldBe(processamento.AnaliseResultado.RiscosArquiteturais.Select(item => item.Valor).ToList());
        mensagem.RecomendacoesBasicas.ShouldBe(processamento.AnaliseResultado.RecomendacoesBasicas.Select(item => item.Valor).ToList());
        mensagem.DataConclusao.ShouldBe(processamento.HistoricoTemporal.DataConclusaoProcessamento!.Value);
    }

    public static void DeveConterDadosErro(this ProcessamentoDiagramaErroDto mensagem, string correlationId, ProcessamentoDiagramaAggregate processamento, string motivo)
    {
        mensagem.CorrelationId.ShouldBe(correlationId);
        mensagem.AnaliseDiagramaId.ShouldBe(processamento.AnaliseDiagramaId);
        mensagem.Motivo.ShouldBe(motivo);
        mensagem.TentativasRealizadas.ShouldBe(processamento.TentativasProcessamento.Valor);
        mensagem.DataErro.ShouldBe(processamento.HistoricoTemporal.DataConclusaoProcessamento!.Value);
    }
}
