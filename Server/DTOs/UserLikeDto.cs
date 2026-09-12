namespace Server.DTOs;

public class UserLikeDto
{
    public int Id { get; set; }
    public required string DisplayName { get; set; }
    public DateOnly DateOfBirth { get; set; }
    public string City { get; set; } = null!;
    public string? ImageUrl { get; set; }
}