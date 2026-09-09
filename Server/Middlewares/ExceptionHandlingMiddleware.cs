using Server.Core.Exceptions;

namespace Server.Middlewares;

public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (AppExceptions e)
        {
            context.Response.StatusCode = e.StatusCode;
            await context.Response.WriteAsJsonAsync(new { message = e.Message });
        }
        catch (Exception e)
        {
            logger.LogError(e, "An unhandled exception occurred.");

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsJsonAsync(new { message = "Internal server error" });
        }
    }
}