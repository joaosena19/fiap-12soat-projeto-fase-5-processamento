using Shared.Attributes;
using Shared.Enums;
using Shared.Exceptions;

namespace Domain.ProcessamentoDiagrama.ValueObjects;

[ValueObject]
public record RiscoArquitetural
{
    private readonly string _valor = string.Empty;

    private RiscoArquitetural() { }

    public RiscoArquitetural(string valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
            throw new DomainException("Risco arquitetural não pode ser vazio", ErrorType.InvalidInput);

        _valor = valor.Trim();
    }

    public string Valor => _valor;
}