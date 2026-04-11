```csharp
public class CreatePriceInquiryCommandBuilder
{
    private Guid userId = Guid.NewGuid();
    private string title = "Cotação de materiais de escritório";
    private string description = "Busco fornecedores de materiais de escritório com bom preço e entrega rápida";
    private Guid categoryId = Guid.NewGuid();
    private List<CreatePriceInquiryItemCommand> items = [new("Caneta esferográfica", 100)];

    public CreatePriceInquiryCommandBuilder ComUserId(Guid value) { userId = value; return this; }
    public CreatePriceInquiryCommandBuilder ComTitulo(string value) { title = value; return this; }
    public CreatePriceInquiryCommandBuilder ComDescricao(string value) { description = value; return this; }
    public CreatePriceInquiryCommandBuilder ComCategoriaId(Guid value) { categoryId = value; return this; }

    public CreatePriceInquiryCommand Build() => new(userId, title, description, categoryId, null, null, null, null, new DateTimeOffset(2099, 12, 31, 23, 59, 59, TimeSpan.Zero), items);
}
```
