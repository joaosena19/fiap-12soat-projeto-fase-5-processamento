```csharp
[Fact(DisplayName = "Deve retornar falha quando consulta de preços não existe")]
[Trait("Category", "Unit")]
[Trait("Priority", "High")]
[Trait("Handle", "PriceInquiryNotFound")]
public async Task Handle_ConsultaNaoExiste_DeveRetornarFalhaAsync()
{
}
```
