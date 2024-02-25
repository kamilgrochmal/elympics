namespace ElympicsNet.Api.Exceptions;

public abstract class ElympicsNetException : Exception
{
    protected ElympicsNetException(string message) : base(message)
    {
        
    }
}
