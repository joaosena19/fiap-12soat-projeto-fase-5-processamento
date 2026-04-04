namespace Infrastructure.LLM;

public class LlmPermanentException : Exception
{
    public LlmPermanentException(string message) : base(message)
    {
    }

    public LlmPermanentException(string message, Exception innerException) : base(message, innerException)
    {
    }
}