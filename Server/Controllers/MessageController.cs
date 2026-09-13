using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Server.Core.Paginations;
using Server.DTOs;
using Server.Services;

namespace Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MessageController(MessageService messageService) : ControllerBase
{
    private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<ActionResult<PaginatedResult<MessageDto>>> GetMessages([FromQuery] MessageParams messageParams)
    {
        messageParams.UserId = GetUserId();
        return Ok(await messageService.GetMessagesAsync(messageParams));
    }

    [HttpGet("thread/{recipientId}")]
    public async Task<ActionResult<IReadOnlyList<MessageDto>>> GetMessageThread(int recipientId)
    {
        return Ok(await messageService.GetMessageThreadAsync(GetUserId(), recipientId));
    }

    [HttpPost]
    public async Task<ActionResult<MessageDto>> CreateMessage(MessageCreateDto dto)
    {
        return StatusCode(StatusCodes.Status201Created, await messageService.AddMessageAsync(GetUserId(), dto));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteMessage(int id)
    {
        await messageService.DeleteMessageAsync(GetUserId(), id);
        return NoContent();
    }
}