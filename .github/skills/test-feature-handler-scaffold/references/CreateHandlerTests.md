```csharp
public class CreatePriceInquiryCommandHandlerTests
{
    private readonly PriceInquiryTestFixture fixture;

    public CreatePriceInquiryCommandHandlerTests()
    {
        fixture = new PriceInquiryTestFixture();
    }

    #region Casos de Sucesso

    [Fact(DisplayName = "Deve criar consulta de preços com sucesso quando dados são válidos")]
    [Trait("Category", "Unit")]
    [Trait("Priority", "High")]
    [Trait("Handle", "ValidData")]
    public async Task Handle_DadosValidos_CriaConsultaDePrecoComSucessoAsync()
    {
        // Arrange
        var command = new CreatePriceInquiryCommandBuilder().Build();

        // Act
        var result = await fixture.CreatePriceInquiryHandler.Handle(command, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
    }

    #endregion
}
```
