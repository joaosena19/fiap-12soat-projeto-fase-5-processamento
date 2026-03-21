using Domain.ProcessamentoDiagrama.Enums;
using Shared.Attributes;
using Shared.Enums;
using Shared.Exceptions;

namespace Domain.ProcessamentoDiagrama.ValueObjects;

[ValueObject]
public record StatusProcessamento
{
    private readonly StatusProcessamentoEnum _valor;

    private StatusProcessamento() { }

    public StatusProcessamento(StatusProcessamentoEnum valor)
    {
        if (!Enum.IsDefined(valor))
        {
            var valores = string.Join(", ", Enum.GetNames<StatusProcessamentoEnum>());
            throw new DomainException($"Status de processamento '{valor}' não é válido. Valores aceitos: {valores}.", ErrorType.InvalidInput);
        }

        _valor = valor;
    }

    public StatusProcessamentoEnum Valor => _valor;
}