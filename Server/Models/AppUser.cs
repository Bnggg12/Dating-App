using Microsoft.AspNetCore.Identity;
using Server.Helpers;

namespace Server.Models;

public class AppUser : IdentityUser<int>
{
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiry { get; set; }

    public string DisplayName { get; set; } = null!;
    public DateOnly DateOfBirth { get; set; }
    public string Gender { get; set; }  = null!;
    public string? LookingFor { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime LastActive { get; set; }
    public string Description { get; set; } = null!;
    public string City { get; set; } = null!;

    public string? Mbti { get; set; }
    public string? EducationLevel { get; set; }
    public string? FieldOfStudy { get; set; }
    public string? Institution { get; set; }

    public List<string> Interests { get; set; } = [];
    public List<Photo> Photos { get; set; } = [];
    
    public List<AppUserRole> UserRoles { get; set; } = [];

    public List<UserLike> LikeByMembers { get; set; } = [];
    public List<UserLike> LikedMembers { get; set; } = [];

    public List<Message> MessagesSent { get; set; } = [];
    public List<Message> MessagesReceived { get; set; } = [];
}