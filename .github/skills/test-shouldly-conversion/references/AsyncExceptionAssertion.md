```csharp
public async Task Handle_ConsultaNaoExiste_DeveRetornarFalhaAsync()
{
    // Arrange
    var result = await fixture.GetPriceInquiryProposalsHandler.Handle(query, CancellationToken.None);

    // Assert
    result.IsFailure.ShouldBeTrue();
    result.Errors.ShouldNotBeNull();
    result.Errors!.Any(error => error.Code == PriceInquiryErrors.NotFound).ShouldBeTrue();
}
```
