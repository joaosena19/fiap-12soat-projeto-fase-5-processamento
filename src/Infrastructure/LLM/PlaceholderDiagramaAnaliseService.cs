using Application.Contracts.LLM;

namespace Infrastructure.LLM;

/// <summary>
/// Implementação placeholder do serviço de análise de diagramas via LLM.
/// </summary>
public class PlaceholderDiagramaAnaliseService : IDiagramaAnaliseClient
{
    public async Task<ResultadoAnaliseDto> AnalisarDiagramaAsync(string nomeFisico, byte[] conteudoArquivo, string extensao)
    {
        await Task.Delay(500);

        return new ResultadoAnaliseDto
        {
            Sucesso = true,
            DescricaoAnalise = $"Análise automática do diagrama '{nomeFisico}'. O diagrama apresenta uma arquitetura de microsserviços com comunicação assíncrona via mensageria. Foram identificados os principais componentes do sistema, incluindo APIs, bancos de dados e filas de mensagens.",
            ComponentesIdentificados = new List<string>
            {
                $"API principal do domínio modelada em {extensao}",
                "Fila assíncrona para processamento desacoplado",
                "Banco de dados relacional para persistência do fluxo",
                "Serviço de mensageria para integração entre componentes"
            },
            RiscosArquiteturais = new List<string>
            {
                "Dependência excessiva de integração síncrona entre serviços críticos",
                "Ausência de estratégia explícita para idempotência de consumo",
                "Observabilidade insuficiente em falhas de integração externas"
            },
            RecomendacoesBasicas = new List<string>
            {
                "Definir contratos versionados para eventos de domínio publicados",
                "Adicionar políticas de idempotência nos consumidores de fila",
                "Ampliar métricas e logs de correlação ponta a ponta"
            },
            TentativasRealizadas = 1
        };
    }
}
