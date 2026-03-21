using Shared.Attributes;
using Shared.Enums;
using Shared.Exceptions;

namespace Domain.ProcessamentoDiagrama.ValueObjects;

[ValueObject]
public record ComponenteIdentificado
{
    private readonly string _valor = string.Empty;

    private ComponenteIdentificado() { }

    public ComponenteIdentificado(string valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
            throw new DomainException("Componente identificado não pode ser vazio", ErrorType.InvalidInput);

        _valor = valor.Trim();
    }

    public string Valor => _valor;
}