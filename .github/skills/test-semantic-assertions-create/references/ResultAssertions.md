```csharp
public static class PriceInquiryAssertionExtensions
{
    public static void DeveRefletirOsDadosDoComando(this CreatePriceInquiryResult result, CreatePriceInquiryCommand command)
    {
        result.ShouldNotBeNull();
        result.Title.ShouldBe(command.Title);
        result.Description.ShouldBe(command.Description);
        result.UserId.ShouldBe(command.UserId);
        result.CategoryId.ShouldBe(command.CategoryId);
    }
}
```
