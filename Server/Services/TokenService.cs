using Microsoft.AspNetCore.Identity;
using Server.Models;

namespace Server.Services;

public class TokenService(IConfiguration config, UserManager<AppUser> userManager)
{
    public async Task<string> CreateToken(AppUser user)
    {
        
    }

    public string GenerateRefreshToken()
    {
        
    }
}