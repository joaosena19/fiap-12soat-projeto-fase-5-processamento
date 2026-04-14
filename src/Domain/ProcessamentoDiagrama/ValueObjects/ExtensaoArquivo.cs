using Shared.Attributes;
using Shared.Enums;
using Shared.Exceptions;

namespace Domain.ProcessamentoDiagrama.ValueObjects;

[ValueObject]
public record ExtensaoArquivo
{
    private const int ComprimentoMaximo = 20;
    private readonly string _valor = string.Empty;

    private ExtensaoArquivo() { }

    public ExtensaoArquivo(string valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
            throw new DomainException("Extensão do arquivo não pode ser vazia", ErrorType.InvalidInput);

        valor = valor.Trim();

        if (valor.Length > ComprimentoMaximo)
            throw new DomainException($"Extensão do arquivo não pode exceder {ComprimentoMaximo} caracteres", ErrorType.InvalidInput);

        _valor = valor;
    }

    public string Valor => _valor;
}
