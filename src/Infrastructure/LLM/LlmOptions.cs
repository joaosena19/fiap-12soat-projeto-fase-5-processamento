namespace Infrastructure.LLM;

public sealed class LlmOptions
{
    public string ApiKey { get; init; } = string.Empty;
    public List<string> Modelos { get; init; } = [];
}
