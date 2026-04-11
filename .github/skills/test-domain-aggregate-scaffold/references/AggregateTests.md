```csharp
public class PriceInquiryAggregateTests
{
    [Fact(DisplayName = "Deve lançar exceção quando lista de itens está vazia")]
    [Trait("Aggregate", "PriceInquiry")]
    [Trait("Category", "Unit")]
    public void Create_SemItens_DeveLancarExcecao()
    {
        // Arrange
        Func<PriceInquiryAgg> action = () => new PriceInquiryBuilder().SemItens().Build();

        // Act & Assert
        action.DeveLancarExcecaoDeValidacaoExata("At least one item is required");
    }
}
```
