using Shared.Attributes;
using Shared.Enums;
using Shared.Exceptions;

namespace Domain.ProcessamentoDiagrama.ValueObjects;

[ValueObject]
public record RecomendacaoBasica
{
    private readonly string _valor = string.Empty;

    private RecomendacaoBasica() { }

    public RecomendacaoBasica(string valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
            throw new DomainException("Recomendação básica não pode ser vazia", ErrorType.InvalidInput);

        _valor = valor.Trim();
    }

    public string Valor => _valor;
}