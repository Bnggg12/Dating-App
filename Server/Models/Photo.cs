namespace Server.Models;

public class Photo
{
    public int Id { get; set; }
    public string Url { get; set; } = null!;
    public bool IsMain { get; set; }
    public bool IsApproved { get; set; } = false;
    public string? PublicId { get; set; }

    public int UserId { get; set; }
    public AppUser User { get; set; } = null!;
}