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

    public static AnaliseResultado Criar(DescricaoAnalise descricaoAnalise, List<ComponenteIdentificado> componentesIdentificados, List<RiscoArquitetural> riscosArquiteturais, List<RecomendacaoBasica> recomendacoesBasicas)
    {
        return new AnaliseResultado(descricaoAnalise, componentesIdentificados, riscosArquiteturais, recomendacoesBasicas);
    }
}