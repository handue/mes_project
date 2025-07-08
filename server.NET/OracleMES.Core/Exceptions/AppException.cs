namespace OracleMES.Core.Exceptions;

public class AppException : Exception
{
    public string ErrorCode { get; }
    public object[] Parameters { get; }

    public AppException(string message, string errorCode, params object[] parameters) : base(message)
    {
        ErrorCode = errorCode;
        Parameters = parameters;
    }
}