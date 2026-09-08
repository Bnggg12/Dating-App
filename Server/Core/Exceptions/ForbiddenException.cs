namespace Server.Core.Exceptions;

public class ForbiddenException(string message) : AppExceptions(message)
{
    public override int StatusCode => StatusCodes.Status403Forbidden;
}