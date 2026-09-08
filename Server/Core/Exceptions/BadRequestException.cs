namespace Server.Core.Exceptions;

public class BadRequestException(string message) : AppExceptions(message)
{
    public override int StatusCode => StatusCodes.Status400BadRequest;
}