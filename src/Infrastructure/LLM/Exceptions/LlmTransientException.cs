namespace Infrastructure.LLM;

public class LlmTransientException : Exception
{
    public LlmTransientException(string message) : base(message)
    {
    }

    public LlmTransientException(string message, Exception innerException) : base(message, innerException)
    {
    }
}