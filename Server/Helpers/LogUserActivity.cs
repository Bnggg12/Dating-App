using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Server.Data;

namespace Server.Helpers;
public class LogUserActivity : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var resultContext = await next();

        if (context.HttpContext.User.Identity?.IsAuthenticated != true) return;
    
        var userId = int.Parse(resultContext.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var db = resultContext.HttpContext.RequestServices.GetRequiredService<AppDbContext>();

        await db.Users
            .Where(x => x.Id == userId)
            .ExecuteUpdateAsync(setters => setters.SetProperty(x => x.LastActive, TimeHelper.NowVN()));
    }
}