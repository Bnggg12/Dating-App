using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Server.Core.Exceptions;
using Server.DTOs;
using Server.Helpers;
using Server.Mappers;
using Server.Models;

namespace Server.Services;

public class AccountService(UserManager<AppUser> userManager, TokenService tokenService, IHttpContextAccessor httpContextAccessor)
{
    public async Task<(IdentityResult? Errors, UserDto? User)> RegisterAsync(RegisterDto dto)
    {
        var emailExists = await userManager.Users.AnyAsync(x => x.Email == dto.Email.ToLower());
        if (emailExists) throw new BadRequestException("Email already exists");

        var user = UserMapper.ToEntity(dto);

        var result = await userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded) return (result, null);

        await userManager.AddToRoleAsync(user, "Member");

        await SetRefreshTokenCookie(user);

        return (null, await UserMapper.ToDto(user, tokenService, ["Member"]));
    }

    public async Task<UserDto> LoginAsync(LoginDto dto)
    {
        var user = await userManager.Users
            .Include(u => u.Photos)
            .FirstOrDefaultAsync(x => x.Email == dto.Email.ToLower())
            ?? throw new UnauthorizedException("Invalid email address");
        
        var result = await userManager.CheckPasswordAsync(user, dto.Password);
        if (!result) throw new UnauthorizedException("Invalid password");

        await SetRefreshTokenCookie(user);

        var roles = await userManager.GetRolesAsync(user);

        return await UserMapper.ToDto(user, tokenService, [.. roles]);
    }

    public async Task<UserDto> RefreshTokenAsync()
    {
        var refreshToken = httpContextAccessor.HttpContext?.Request.Cookies["refreshToken"];
        if (string.IsNullOrEmpty(refreshToken))
            throw new UnauthorizedException("Refresh token not found");

        var user = await userManager.Users
            .Include(u => u.Photos)
            .FirstOrDefaultAsync(x => x.RefreshToken == refreshToken
            && x.RefreshTokenExpiry > TimeHelper.NowVN())
            ?? throw new UnauthorizedException("The session has expired");
        
        await SetRefreshTokenCookie(user);

        var roles = await userManager.GetRolesAsync(user);

        return await UserMapper.ToDto(user, tokenService, [.. roles]);
    }

    public async Task LogoutAsync()
    {
        var userId = httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!string.IsNullOrEmpty(userId))
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user != null)
            {
                user.RefreshToken = null;
                user.RefreshTokenExpiry = null;
                await userManager.UpdateAsync(user);
            }
        }

        httpContextAccessor.HttpContext?.Response.Cookies.Delete("refreshToken");
    }

    private async Task SetRefreshTokenCookie(AppUser user)
    {
        var refreshToken = tokenService.GenerateRefreshToken();
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiry = TimeHelper.NowVN().AddDays(1);
        await userManager.UpdateAsync(user);

        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = TimeHelper.NowVN().AddDays(1)
        };

        httpContextAccessor.HttpContext?.Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
    }
}