using Domain.ProcessamentoDiagrama.ValueObjects;
using Shared.Attributes;

namespace Domain.ProcessamentoDiagrama.Entities;

[AggregateMember]
public class AnaliseResultado
{
    public DescricaoAnalise DescricaoAnalise { get; private set; } = null!;
    public List<ComponenteIdentificado> ComponentesIdentificados { get; private set; } = new();
    public List<RiscoArquitetural> RiscosArquiteturais { get; private set; } = new();
    public List<RecomendacaoBasica> RecomendacoesBasicas { get; private set; } = new();

    private AnaliseResultado() { }

    private AnaliseResultado(DescricaoAnalise descricaoAnalise, List<ComponenteIdentificado> componentesIdentificados, List<RiscoArquitetural> riscosArquiteturais, List<RecomendacaoBasica> recomendacoesBasicas)
    {
        DescricaoAnalise = descricaoAnalise;
        ComponentesIdentificados = componentesIdentificados;
        RiscosArquiteturais = riscosArquiteturais;
        RecomendacoesBasicas = recomendacoesBasicas;
    }

    public static AnaliseResultado Criar(string descricaoAnalise, List<string> componentesIdentificados, List<string> riscosArquiteturais, List<string> recomendacoesBasicas)
    {
        return new AnaliseResultado(
            new DescricaoAnalise(descricaoAnalise),
            componentesIdentificados.Select(item => new ComponenteIdentificado(item)).ToList(),
            riscosArquiteturais.Select(item => new RiscoArquitetural(item)).ToList(),
            recomendacoesBasicas.Select(item => new RecomendacaoBasica(item)).ToList());
    }
}