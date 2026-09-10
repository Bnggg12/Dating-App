using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Core.Constants;
using Server.DTOs;
using Server.Services;

namespace Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountController(AccountService accountService) : ControllerBase
{
    [HttpPost("register")]
    public async Task<ActionResult<UserDto>> Register(RegisterDto dto)
    {
        var (errors, user) = await accountService.RegisterAsync(dto);

        if (errors != null)
        {
            foreach (var error in errors.Errors)
            {
                ModelState.AddModelError("identity", error.Description);
            }

            return ValidationProblem();
        }

        return Ok(user);
    }

    [HttpPost("login")]
    public async Task<ActionResult<UserDto>> Login(LoginDto dto)
    {
        return Ok(await accountService.LoginAsync(dto));
    }

    [HttpPost("refresh-token")]
    public async Task<ActionResult<UserDto>> RefreshToken()
    {
        return Ok(await accountService.RefreshTokenAsync());
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await accountService.LogoutAsync();
        return NoContent();
    }

    [HttpGet("mbtis")]
    public ActionResult<IReadOnlyList<string>> GetMbtis()
    {
        return Ok(AppConstants.MbtiList);
    }

    [HttpGet("edu-levels")]
    public ActionResult<IReadOnlyList<string>> GetEduLevels()
    {
        return Ok(AppConstants.EducationLevels);
    }

    [HttpGet("cities")]
    public ActionResult<IReadOnlyList<string>> GetCities()
    {
        return Ok(AppConstants.Cities);
    }
}