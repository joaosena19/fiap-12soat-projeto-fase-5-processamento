namespace Tests.Helpers.Extensions;

public static class DomainExceptionAssertionExtensions
{
    public static DomainException DeveLancarExcecaoDeValidacao(this Action action, string mensagemParcial)
    {
        var excecao = Should.Throw<DomainException>(action);
        excecao.Message.ShouldContain(mensagemParcial);
        return excecao;
    }

    public static DomainException DeveLancarExcecaoDeValidacaoExata(this Action action, string mensagemExata)
    {
        var excecao = Should.Throw<DomainException>(action);
        excecao.Message.ShouldBe(mensagemExata);
        return excecao;
    }

    public static DomainException DeveLancarExcecaoDeValidacao(this Func<object> action, string mensagemParcial)
    {
        var excecao = Should.Throw<DomainException>(action);
        excecao.Message.ShouldContain(mensagemParcial);
        return excecao;
    }

    public static DomainException DeveLancarExcecaoDeValidacaoExata(this Func<object> action, string mensagemExata)
    {
        var excecao = Should.Throw<DomainException>(action);
        excecao.Message.ShouldBe(mensagemExata);
        return excecao;
    }
}
