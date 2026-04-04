```csharp
public class PriceInquiryTestFixture
{
    public Mock<IPriceInquiryRepository> PriceInquiryRepositoryMock { get; }
    public Mock<ICommunityValidationService> CommunityValidationServiceMock { get; }
    public Mock<IUserManagementService> UserManagementServiceMock { get; }

    public PriceInquiryTestFixture ComUsuarioValido(Guid userId)
    {
        UserManagementServiceMock.AoObterUsuarioPorId(userId).Retorna(CriarUsuarioValido(userId));
        return this;
    }

    public PriceInquiryTestFixture ComCategoriaInexistente()
    {
        CommunityValidationServiceMock.AoVerificarCategorias().Retorna(false);
        return this;
    }
}
```
