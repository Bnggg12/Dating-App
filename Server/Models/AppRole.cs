using Microsoft.AspNetCore.Identity;

namespace Server.Models;

public class AppRole : IdentityRole<int>
{
    public List<AppUserRole> UserRoles { get; set; } = [];
}