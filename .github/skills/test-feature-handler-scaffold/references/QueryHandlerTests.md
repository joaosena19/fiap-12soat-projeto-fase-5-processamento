```csharp
public class GetPriceInquiryByIdQueryHandlerTests
{
    private readonly PriceInquiryTestFixture fixture;

    public GetPriceInquiryByIdQueryHandlerTests()
    {
        fixture = new PriceInquiryTestFixture();
    }

    #region Casos de Consulta

    [Fact(DisplayName = "Deve retornar consulta de preços quando id existe")]
    [Trait("Category", "Unit")]
    [Trait("Priority", "High")]
    [Trait("Handle", "Found")]
    public async Task Handle_IdExistente_RetornaConsultaDePrecoAsync()
    {
        // Arrange
        var query = new GetPriceInquiryByIdQueryBuilder().Build();

        // Act
        var result = await fixture.GetPriceInquiryByIdHandler.Handle(query, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
    }

    #endregion
}
```
