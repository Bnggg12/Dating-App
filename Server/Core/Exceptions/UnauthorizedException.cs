namespace Server.Core.Exceptions;

public class UnauthorizedException(string message) : AppExceptions(message)
{
    public override int StatusCode => StatusCodes.Status401Unauthorized;
}