namespace Server.Models;

public class UserLike
{
    public int SourceMemberId { get; set; }
    public AppUser SourceMember { get; set; } = null!;
    public int TargetMemberId { get; set; }
    public AppUser TargetMember { get; set; } = null!;
}