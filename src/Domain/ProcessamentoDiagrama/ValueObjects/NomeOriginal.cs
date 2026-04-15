using Shared.Attributes;
using Shared.Enums;
using Shared.Exceptions;

namespace Domain.ProcessamentoDiagrama.ValueObjects;

[ValueObject]
public record NomeOriginal
{
    private const int ComprimentoMaximo = 500;
    private readonly string _valor = string.Empty;

    private NomeOriginal() { }

    public NomeOriginal(string valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
            throw new DomainException("Nome original não pode ser vazio", ErrorType.InvalidInput);

        valor = valor.Trim();

        if (valor.Length > ComprimentoMaximo)
            throw new DomainException($"Nome original não pode exceder {ComprimentoMaximo} caracteres", ErrorType.InvalidInput);

        _valor = valor;
    }

    public string Valor => _valor;
}
