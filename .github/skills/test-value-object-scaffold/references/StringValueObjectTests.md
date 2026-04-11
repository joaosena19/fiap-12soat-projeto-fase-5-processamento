```csharp
public class DescriptionTests
{
    [Theory(DisplayName = "Deve rejeitar valores nulos, vazios e somente espaços")]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    [Trait("ValueObject", "Description")]
    [Trait("Category", "Unit")]
    public void Create_ComValoresVazios_DeveRetornarFalha(string? valor)
    {
        // Arrange

        // Act
        var result = Description.Create(valor!);

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Errors.ShouldNotBeNull();
        result.Errors!.Count.ShouldBe(1);
        result.Errors[0].Code.ShouldBe(DescriptionErrors.EmptyOrNull);
    }
}
```
