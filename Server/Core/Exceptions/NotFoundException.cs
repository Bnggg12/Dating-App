namespace Server.Core.Exceptions;

public class NotFoundException(string message) : AppExceptions(message)
{
    public override int StatusCode => StatusCodes.Status404NotFound;
}