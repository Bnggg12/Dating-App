namespace Server.DTOs;

public class UserRoleDto
{
    public int UserId { get; set; }
    public string Email { get; set; } = null!;
    public string DisplayName { get; set; } = null!;
    public string? ImageUrl { get; set; }
    public List<string> Roles { get; set; } = [];
}