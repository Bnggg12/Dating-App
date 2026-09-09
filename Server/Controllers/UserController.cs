using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.DTOs;
using Server.Services;

namespace Server.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class UserController(UserService userService) : ControllerBase
{
    [HttpGet("{id}")]
    public async Task<ActionResult<UserProfileDto>> GetById(int id)
    {
        return Ok(await userService.GetByIdAsync(id));
    }

    [HttpPut]
    public async Task<IActionResult> Update(UserUpdateDto dto)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        await userService.UpdateAsync(userId, dto);
        return NoContent();
    }
}