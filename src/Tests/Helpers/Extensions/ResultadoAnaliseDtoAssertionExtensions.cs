using Application.Contracts.LLM;

namespace Tests.Helpers.Extensions;

public static class ResultadoAnaliseDtoAssertionExtensions
{
    public static void DeveSerSucesso(this ResultadoAnaliseDto resultado, int tentativasEsperadas)
    {
        resultado.Sucesso.ShouldBeTrue();
        resultado.TentativasRealizadas.ShouldBe(tentativasEsperadas);
    }

    public static void DeveSerFalha(this ResultadoAnaliseDto resultado, string motivoContendo, int tentativasEsperadas)
    {
        resultado.Sucesso.ShouldBeFalse();
        resultado.MotivoErro.ShouldNotBeNull();
        resultado.MotivoErro.ShouldContain(motivoContendo);
        resultado.TentativasRealizadas.ShouldBe(tentativasEsperadas);
    }
}
