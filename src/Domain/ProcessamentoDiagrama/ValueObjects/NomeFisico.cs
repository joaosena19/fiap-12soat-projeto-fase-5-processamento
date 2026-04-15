using Shared.Attributes;
using Shared.Enums;
using Shared.Exceptions;

namespace Domain.ProcessamentoDiagrama.ValueObjects;

[ValueObject]
public record NomeFisico
{
    private const int ComprimentoMaximo = 200;
    private readonly string _valor = string.Empty;

    private NomeFisico() { }

    public NomeFisico(string valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
            throw new DomainException("Nome físico não pode ser vazio", ErrorType.InvalidInput);

        valor = valor.Trim();

        if (valor.Length > ComprimentoMaximo)
            throw new DomainException($"Nome físico não pode exceder {ComprimentoMaximo} caracteres", ErrorType.InvalidInput);

        _valor = valor;
    }

    public string Valor => _valor;
}
