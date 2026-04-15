namespace Infrastructure.LLM;

public class LlmIndisponivelException : LlmTransientException
{
    public int? CodigoHttp { get; }
    public string Modelo { get; }

    public LlmIndisponivelException(string modelo, int? codigoHttp, string message, Exception? innerException = null) : base(message, innerException!)
    {
        Modelo = modelo;
        CodigoHttp = codigoHttp;
    }
}
