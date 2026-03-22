namespace Infrastructure.LLM;

/// <summary>
/// Prompts utilizados na análise de diagramas de arquitetura via LLM.
/// </summary>
public static class DiagramaAnalisePrompts
{
    public const string SystemPrompt = """
        Você é um Arquiteto de Software Sênior.
        Sua tarefa é analisar o diagrama de arquitetura fornecido e extrair as informações solicitadas.
        Seja técnico, objetivo e não invente componentes que não estão visíveis na imagem.
        """;

    public const string UserPrompt = """
        Analise este diagrama de arquitetura de software e preencha o relatório estruturado.

        Regras:
        - DescricaoAnalise: parágrafo com visão geral da arquitetura identificada no diagrama.
        - ComponentesIdentificados: liste TODOS os componentes visíveis (APIs, bancos, filas, gateways, serviços, etc.). Entre 3 e 10 itens.
        - RiscosArquiteturais: identifique riscos reais baseados no diagrama (ponto único de falha, acoplamento, falta de resiliência, etc.). Entre 3 e 10 itens.
        - RecomendacoesBasicas: sugestões práticas e acionáveis para melhorar a arquitetura. Entre 3 e 10 itens.
        """;
}
