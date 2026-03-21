using Shared.Attributes;
using Shared.Enums;
using Shared.Exceptions;

namespace Domain.ProcessamentoDiagrama.ValueObjects;

[ValueObject]
public record DescricaoAnalise
{
    private readonly string _valor = string.Empty;

    private DescricaoAnalise() { }

    public DescricaoAnalise(string valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
            throw new DomainException("Descrição da análise não pode ser vazia", ErrorType.InvalidInput);

        if (valor.Length > 10000)
            throw new DomainException("Descrição da análise não pode exceder 10000 caracteres", ErrorType.InvalidInput);

        _valor = valor.Trim();
    }

    public string Valor => _valor;
}
