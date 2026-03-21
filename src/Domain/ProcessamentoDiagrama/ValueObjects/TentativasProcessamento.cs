using Shared.Attributes;
using Shared.Enums;
using Shared.Exceptions;

namespace Domain.ProcessamentoDiagrama.ValueObjects;

[ValueObject]
public record TentativasProcessamento
{
    private readonly int _valor;

    private TentativasProcessamento() { }

    public TentativasProcessamento(int valor)
    {
        if (valor < 0)
            throw new DomainException("Tentativas de processamento não podem ser negativas", ErrorType.InvalidInput);

        _valor = valor;
    }

    public int Valor => _valor;

    public TentativasProcessamento Incrementar()
    {
        return new TentativasProcessamento(_valor + 1);
    }
}