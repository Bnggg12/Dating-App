using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.DTOs;
using Server.Services;

namespace Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AdminController(AdminService adminService) : ControllerBase
{
    [Authorize(Policy = "RequireAdminRole")]
    [HttpGet("users-with-roles")]
    public async Task<ActionResult<IEnumerable<UserRoleDto>>> GetUserWithRoles()
    {
        return Ok(await adminService.GetUserWithRolesAsync());
    }

    [Authorize(Policy = "RequireAdminRole")]
    [HttpPost("edit-roles/{userId}")]
    public async Task<ActionResult<IList<string>>> EditRoles(int userId, [FromQuery] string roles)
    {
        return Ok(await adminService.EditRolesAsync(userId, roles));
    }

    [Authorize(Policy = "RequireModerateRole")]
    [HttpGet("photos-for-approve")]
    public async Task<ActionResult<IEnumerable<ApprovePhotoDto>>> GetPhotosForApprove()
    {
        return Ok(await adminService.GetPhotosForApproveAsync());
    }

    [Authorize(Policy = "RequireModerateRole")]
    [HttpPut("approve-photo/{photoId}")]
    public async Task<ActionResult> ApprovePhoto(int photoId)
    {
        await adminService.ApprovePhotoAsync(photoId);
        return NoContent();
    }

    [Authorize(Policy = "RequireModerateRole")]
    [HttpDelete("reject-photo/{photoId}")]
    public async Task<ActionResult> RejectPhoto(int photoId)
    {
        await adminService.RejectPhotoAsync(photoId);
        return NoContent();
    }
}