using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.DTOs;
using Server.Core.Paginations;
using Server.Services;

namespace Server.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class LikeController(LikeService likeService) : ControllerBase
{   
    private int GetCurrentUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<ActionResult<PaginatedResult<UserLikeDto>>> GetUserLikes([FromQuery] LikesParams likesParams)
    {
        likesParams.UserId = GetCurrentUserId();
        return Ok(await likeService.GetUserLikes(likesParams));
    }

    [HttpGet("list")]
    public async Task<ActionResult<IEnumerable<int>>> GetCurrentUserLikeIds()
    {
        return Ok(await likeService.GetCurrentUserLikeIds(GetCurrentUserId()));
    }

    [HttpPost("{targetUserId:int}")]
    public async Task<ActionResult> ToggleLike(int targetUserId)
    {
        await likeService.ToggleLike(GetCurrentUserId(), targetUserId);
        return Ok();
    }
}