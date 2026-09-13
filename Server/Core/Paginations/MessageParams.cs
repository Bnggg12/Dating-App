namespace Server.Core.Paginations;

public class MessageParams : PagingParams
{
    public int UserId { get; set; }
    public string Container { get; set; } = "Inbox";
}