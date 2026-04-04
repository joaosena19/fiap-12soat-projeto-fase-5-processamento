```csharp
// Exemplo 1: AAA completo (Arrange, Act, Assert)
[Fact(DisplayName = "Deve criar consulta de preços com sucesso quando dados são válidos")]
public async Task Handle_DadosValidos_CriaConsultaDePrecoComSucessoAsync()
{
    // Arrange
    var command = new CreatePriceInquiryCommandBuilder().Build();

    // Act
    var result = await fixture.CreatePriceInquiryHandler.Handle(command, CancellationToken.None);

    // Assert
    result.ShouldNotBeNull();
}

// Exemplo 2: Arrange & Act combinado (setup e ação na mesma linha)
[Fact(DisplayName = "Deve retornar falha quando título estiver vazio")]
public void Create_TituloVazio_DeveRetornarFalha()
{
    // Arrange & Act
    var result = Title.Create(" ");

    // Assert
    result.IsFailure.ShouldBeTrue();
    result.Errors.ShouldNotBeNull();
    result.Errors!.Any(error => error.Code == TitleErrors.EmptyOrNull).ShouldBeTrue();
}

// Exemplo 3: Act & Assert combinado (chamada e verificação juntas)
[Theory(DisplayName = "Deve retornar falha quando hierarquia de localização for inválida")]
[InlineData(null, 1, null)]
[InlineData(1, null, 1)]
public void Create_LocalizacaoInvalida_DeveRetornarFalha(int? countryId, int? stateId, int? cityId)
{
    // Act & Assert
    var result = PriceInquiryLocation.Create(countryId, stateId, cityId);

    result.IsFailure.ShouldBeTrue();
}

// Exemplo 4: sem Arrange (não inventar comentário)
[Fact(DisplayName = "Deve gerar id válido")]
public void New_DeveCriarIdValido()
{
    // Act
    var id = PriceInquiryId.New();

    // Assert
    id.Value.ShouldNotBe(Guid.Empty);
}

// Exemplo 5: sem Act explícito (ação embutida no Assert)
[Fact(DisplayName = "Deve retornar falha quando quantidade estiver acima do máximo")]
public void Create_QuantidadeAcimaDoMaximo_DeveRetornarFalha()
{
    // Arrange
    short quantidadeInvalida = (short)(PriceInquiryItemQuantity.MaxValue + 1);

    // Act & Assert
    var result = PriceInquiryItemQuantity.Create(quantidadeInvalida);

    result.IsFailure.ShouldBeTrue();
}
```
