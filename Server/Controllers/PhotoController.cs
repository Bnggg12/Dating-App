using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.DTOs;
using Server.Services;

namespace Server.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class PhotoController(PhotoService photoService) : ControllerBase
{
    private int GetUserId() => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpPost]
    public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
    {
        return Ok(await photoService.UploadPhotoAsync(file, GetUserId()));
    }

    [HttpPut("set-main/{photoId}")]
    public async Task<IActionResult> SetMainPhoto(int photoId)
    {
        await photoService.SetMainPhotoAsync(photoId, GetUserId());
        return NoContent();
    }

    [HttpDelete("{photoId}")]
    public async Task<IActionResult> DeletePhoto(int photoId)
    {
        await photoService.DeletePhotoAsync(photoId, GetUserId());
        return NoContent();
    }
}