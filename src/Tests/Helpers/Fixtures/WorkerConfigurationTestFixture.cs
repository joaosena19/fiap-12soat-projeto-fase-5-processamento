using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Tests.Helpers.Fixtures;

public class WorkerConfigurationTestFixture
{
    private readonly Dictionary<string, string?> _configuracoes = [];

    public IServiceCollection Services { get; } = new ServiceCollection();

    public WorkerConfigurationTestFixture()
    {
        Services.AddLogging();
    }

    public WorkerConfigurationTestFixture ComConfiguracaoBancoValida()
    {
        _configuracoes["DatabaseConnection:Host"] = "localhost";
        _configuracoes["DatabaseConnection:Port"] = "5432";
        _configuracoes["DatabaseConnection:DatabaseName"] = "processamento";
        _configuracoes["DatabaseConnection:User"] = "postgres";
        _configuracoes["DatabaseConnection:Password"] = "postgres";
        return this;
    }

    public WorkerConfigurationTestFixture SemConfiguracaoBanco(string chave)
    {
        _configuracoes.Remove($"DatabaseConnection:{chave}");
        return this;
    }

    public WorkerConfigurationTestFixture ComConfiguracaoLlmValida()
    {
        _configuracoes["LLM:ApiKey"] = "api-key-teste";
        _configuracoes["LLM:Modelos:0"] = "gemini-test-1";
        _configuracoes["LLM:Modelos:1"] = "gemini-test-2";
        return this;
    }

    public WorkerConfigurationTestFixture SemConfiguracaoLlm(string chave)
    {
        if (chave == "Modelos")
        {
            var modeloKeys = _configuracoes.Keys.Where(k => k.StartsWith("LLM:Modelos:")).ToList();
            foreach (var key in modeloKeys)
                _configuracoes.Remove(key);
        }
        else
        {
            _configuracoes.Remove($"LLM:{chave}");
        }
        return this;
    }

    public WorkerConfigurationTestFixture ComConfiguracaoAwsValida()
    {
        _configuracoes["AWS:Region"] = "us-east-1";
        return this;
    }

    public WorkerConfigurationTestFixture ComCredenciaisAwsValidas()
    {
        _configuracoes["AWS:AccessKeyId"] = "access-key-teste";
        _configuracoes["AWS:SecretAccessKey"] = "secret-key-teste";
        return this;
    }

    public WorkerConfigurationTestFixture ComConfiguracaoMensageriaValida()
    {
        _configuracoes["Mensageria:Topicos:UploadDiagramaConcluido"] = "upload-diagrama-concluido";
        _configuracoes["Mensageria:Filas:UploadDiagramaConcluido"] = "upload-diagrama-concluido";
        _configuracoes["Mensageria:Topicos:ProcessamentoDiagramaIniciado"] = "processamento-diagrama-iniciado";
        _configuracoes["Mensageria:Topicos:ProcessamentoDiagramaAnalisado"] = "processamento-diagrama-analisado";
        _configuracoes["Mensageria:Topicos:ProcessamentoDiagramaErro"] = "processamento-diagrama-erro";
        return this;
    }

    public WorkerConfigurationTestFixture SemConfiguracaoAws(string chave)
    {
        _configuracoes.Remove($"AWS:{chave}");
        return this;
    }

    public IConfiguration BuildConfiguration()
    {
        return new ConfigurationBuilder()
            .AddInMemoryCollection(_configuracoes)
            .Build();
    }

    public ServiceProvider BuildServiceProvider()
    {
        return Services.BuildServiceProvider();
    }
}