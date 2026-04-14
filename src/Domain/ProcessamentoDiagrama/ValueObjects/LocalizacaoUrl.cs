using Shared.Attributes;
using Shared.Enums;
using Shared.Exceptions;

namespace Domain.ProcessamentoDiagrama.ValueObjects;

[ValueObject]
public record LocalizacaoUrl
{
    private const int ComprimentoMaximo = 500;
    private readonly string _valor = string.Empty;

    private LocalizacaoUrl() { }

    public LocalizacaoUrl(string valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
            throw new DomainException("Localização URL não pode ser vazia", ErrorType.InvalidInput);

        valor = valor.Trim();

        if (valor.Length > ComprimentoMaximo)
            throw new DomainException($"Localização URL não pode exceder {ComprimentoMaximo} caracteres", ErrorType.InvalidInput);

        _valor = valor;
    }

    public string Valor => _valor;
}
