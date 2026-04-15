using Amazon.S3;
using Application.Contracts.LLM;
using Application.Contracts.Monitoramento;
using Infrastructure.Armazenamento;
using Infrastructure.Database;
using Infrastructure.LLM;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Tests.Helpers.Fixtures;
using Worker.Configurations;
using MassTransit;

namespace Tests.Worker.Configurations;

public class WorkerConfigurationsTests
{
    [Fact(DisplayName = "Deve registrar contexto de banco quando configuração é válida")]
    [Trait("Worker", "DatabaseConfiguration")]
    public void AddDatabase_DeveRegistrarContexto_QuandoConfiguracaoValida()
    {
        // Arrange
        var fixture = new WorkerConfigurationTestFixture()
            .ComConfiguracaoBancoValida();

        // Act
        var retorno = fixture.Services.AddDatabase(fixture.BuildConfiguration());
        using var provider = fixture.BuildServiceProvider();

        // Assert
        retorno.ShouldBe(fixture.Services);
        provider.GetService<DbContextOptions<AppDbContext>>().ShouldNotBeNull();
    }

    [Fact(DisplayName = "Deve lançar exceção quando configuração de banco está incompleta")]
    [Trait("Worker", "DatabaseConfiguration")]
    public void AddDatabase_DeveLancarExcecao_QuandoConfiguracaoBancoIncompleta()
    {
        // Arrange
        var fixture = new WorkerConfigurationTestFixture()
            .ComConfiguracaoBancoValida()
            .SemConfiguracaoBanco("Password");

        // Act
        var acao = () => fixture.Services.AddDatabase(fixture.BuildConfiguration());

        // Assert
        acao.ShouldThrow<InvalidOperationException>();
    }

    [Theory(DisplayName = "Deve lançar exceção quando qualquer campo obrigatório de banco está ausente")]
    [InlineData("Host")]
    [InlineData("Port")]
    [InlineData("DatabaseName")]
    [InlineData("User")]
    [Trait("Worker", "DatabaseConfiguration")]
    public void AddDatabase_DeveLancarExcecao_QuandoCampoObrigatorioAusente(string chaveAusente)
    {
        // Arrange
        var fixture = new WorkerConfigurationTestFixture()
            .ComConfiguracaoBancoValida()
            .SemConfiguracaoBanco(chaveAusente);

        // Act
        var acao = () => fixture.Services.AddDatabase(fixture.BuildConfiguration());

        // Assert
        acao.ShouldThrow<InvalidOperationException>();
    }

    [Fact(DisplayName = "Deve registrar serviços de LLM quando configuração é válida")]
    [Trait("Worker", "LlmConfiguration")]
    public void AddLlmServices_DeveRegistrarDependencias_QuandoConfiguracaoValida()
    {
        // Arrange
        var fixture = new WorkerConfigurationTestFixture()
            .ComConfiguracaoLlmValida();

        // Act
        var retorno = fixture.Services.AddLlmServices(fixture.BuildConfiguration());
        using var provider = fixture.BuildServiceProvider();

        // Assert
        retorno.ShouldBe(fixture.Services);
        provider.GetService<LlmOptions>().ShouldNotBeNull();
        fixture.Services.Any(descriptor => descriptor.ServiceType == typeof(IDiagramaAnaliseService)).ShouldBeTrue();
        fixture.Services.Any(descriptor => descriptor.ServiceType == typeof(ILlmClientFactory)).ShouldBeTrue();
    }

    [Fact(DisplayName = "Deve lançar exceção quando api key da LLM não está configurada")]
    [Trait("Worker", "LlmConfiguration")]
    public void AddLlmServices_DeveLancarExcecao_QuandoApiKeyNaoConfigurada()
    {
        // Arrange
        var fixture = new WorkerConfigurationTestFixture()
            .ComConfiguracaoLlmValida()
            .SemConfiguracaoLlm("ApiKey");

        // Act
        var acao = () => fixture.Services.AddLlmServices(fixture.BuildConfiguration());

        // Assert
        acao.ShouldThrow<InvalidOperationException>();
    }

    [Fact(DisplayName = "Deve lançar exceção quando modelos da LLM não estão configurados")]
    [Trait("Worker", "LlmConfiguration")]
    public void AddLlmServices_DeveLancarExcecao_QuandoModelosNaoConfigurados()
    {
        // Arrange
        var fixture = new WorkerConfigurationTestFixture()
            .ComConfiguracaoLlmValida()
            .SemConfiguracaoLlm("Modelos");

        // Act
        var acao = () => fixture.Services.AddLlmServices(fixture.BuildConfiguration());

        // Assert
        acao.ShouldThrow<InvalidOperationException>();
    }

    [Fact(DisplayName = "Deve registrar serviços de S3 quando região é válida")]
    [Trait("Worker", "S3Configuration")]
    public void AddS3_DeveRegistrarDependencias_QuandoConfiguracaoValida()
    {
        // Arrange
        var fixture = new WorkerConfigurationTestFixture()
            .ComConfiguracaoAwsValida();

        // Act
        var retorno = fixture.Services.AddS3(fixture.BuildConfiguration());
        using var provider = fixture.BuildServiceProvider();
        using var scope = provider.CreateScope();

        // Assert
        retorno.ShouldBe(fixture.Services);
        provider.GetService<IAmazonS3>().ShouldNotBeNull();
        scope.ServiceProvider.GetService<IArquivoDiagramaDownloader>().ShouldNotBeNull();
    }

    [Fact(DisplayName = "Deve registrar serviços de S3 com credenciais explícitas quando fornecidas")]
    [Trait("Worker", "S3Configuration")]
    public void AddS3_DeveRegistrarDependencias_QuandoCredenciaisExplicitas()
    {
        // Arrange
        var fixture = new WorkerConfigurationTestFixture()
            .ComConfiguracaoAwsValida()
            .ComCredenciaisAwsValidas();

        // Act
        var retorno = fixture.Services.AddS3(fixture.BuildConfiguration());
        using var provider = fixture.BuildServiceProvider();
        using var scope = provider.CreateScope();

        // Assert
        retorno.ShouldBe(fixture.Services);
        provider.GetService<IAmazonS3>().ShouldNotBeNull();
        scope.ServiceProvider.GetService<IArquivoDiagramaDownloader>().ShouldNotBeNull();
    }

    [Fact(DisplayName = "Deve lançar exceção quando região AWS não está configurada")]
    [Trait("Worker", "S3Configuration")]
    public void AddS3_DeveLancarExcecao_QuandoRegiaoNaoConfigurada()
    {
        // Arrange
        var fixture = new WorkerConfigurationTestFixture();

        // Act
        var acao = () => fixture.Services.AddS3(fixture.BuildConfiguration());

        // Assert
        acao.ShouldThrow<InvalidOperationException>();
    }

    [Fact(DisplayName = "Deve registrar accessor de correlation id no monitoramento")]
    [Trait("Worker", "MonitoringConfiguration")]
    public void AddMonitoring_DeveRegistrarAccessor_QuandoInvocado()
    {
        // Arrange
        var fixture = new WorkerConfigurationTestFixture();

        // Act
        var retorno = fixture.Services.AddMonitoring();
        using var provider = fixture.BuildServiceProvider();
        using var scope = provider.CreateScope();

        // Assert
        retorno.ShouldBe(fixture.Services);
        scope.ServiceProvider.GetService<ICorrelationIdAccessor>().ShouldNotBeNull();
    }

    [Fact(DisplayName = "Deve registrar serviços de mensageria quando configuração é válida")]
    [Trait("Worker", "MessagingConfiguration")]
    public async Task AddMessaging_DeveRegistrarServicos_QuandoConfiguracaoValida()
    {
        // Arrange
        var fixture = new WorkerConfigurationTestFixture()
            .ComConfiguracaoAwsValida()
            .ComConfiguracaoMensageriaValida();

        // Act
        var retorno = fixture.Services.AddMessaging(fixture.BuildConfiguration());
        var provider = fixture.BuildServiceProvider();

        // Assert
        retorno.ShouldBe(fixture.Services);
        fixture.Services.Any(d => d.ServiceType == typeof(IBus)).ShouldBeTrue();
        fixture.Services.Any(d => d.ServiceType == typeof(global::Application.Contracts.Messaging.IProcessamentoDiagramaMessagePublisher)).ShouldBeTrue();
        provider.GetService<IBusControl>().ShouldNotBeNull();
        await provider.DisposeAsync();
    }

    [Fact(DisplayName = "Deve falhar ao materializar bus quando região AWS não está configurada")]
    [Trait("Worker", "MessagingConfiguration")]
    public async Task AddMessaging_DeveFalharMaterializacaoBus_QuandoRegiaoAwsNaoConfigurada()
    {
        // Arrange
        var fixture = new WorkerConfigurationTestFixture()
            .ComConfiguracaoMensageriaValida();
        fixture.Services.AddMessaging(fixture.BuildConfiguration());
        var provider = fixture.BuildServiceProvider();

        // Act
        var acao = () => provider.GetRequiredService<IBusControl>();

        // Assert
        acao.ShouldThrow<InvalidOperationException>();
        await provider.DisposeAsync();
    }

}