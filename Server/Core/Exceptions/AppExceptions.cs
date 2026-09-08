namespace  Server.Core.Exceptions;

public abstract class AppExceptions(string message) : Exception(message)
{
    public abstract int StatusCode { get; }
}