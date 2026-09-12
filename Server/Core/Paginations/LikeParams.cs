namespace Server.Core.Paginations;

public class LikesParams : PagingParams
{
    public int UserId { get; set; }
    public string Type { get; set; } = "liked";
}