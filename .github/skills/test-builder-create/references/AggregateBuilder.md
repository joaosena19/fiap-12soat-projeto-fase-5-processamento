```csharp
public class PriceInquiryBuilder
{
    private Guid userId = Guid.NewGuid();
    private string title = "Cotação padrão";
    private string description = "Descrição padrão da consulta";
    private Guid categoryId = Guid.NewGuid();
    private List<PriceInquiryItemDto> items = [new() { Name = "Item válido", Quantity = 10 }];

    public PriceInquiryBuilder ComUserId(Guid value) { userId = value; return this; }
    public PriceInquiryBuilder ComTitulo(string value) { title = value; return this; }
    public PriceInquiryBuilder ComDescricao(string value) { description = value; return this; }
    public PriceInquiryBuilder ComCategoriaId(Guid value) { categoryId = value; return this; }
    public PriceInquiryBuilder SemItens() { items = []; return this; }

    public PriceInquiryAggregate Build() => PriceInquiryAggregate.Create(userId, title, description, categoryId, null, null, null, null, new DateTimeOffset(2099, 12, 31, 23, 59, 59, TimeSpan.Zero), items);
}
```
