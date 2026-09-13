namespace Server.DTOs;

public class MessageDto
{
    public int Id { get; set; }

    public int SenderId { get; set; }
    public required string SenderDisplayName { get; set; }
    public string? SenderImageUrl { get; set; }

    public int RecipientId { get; set; }
    public required string RecipientDisplayName { get; set; }
    public string? RecipientImageUrl { get; set; }

    public required string Content { get; set; }
    public DateTime MessageSent { get; set; }
    public DateTime? DateRead { get; set; }
}

public class MessageCreateDto
{
    public int RecipientId { get; set; }
    public required string Content { get; set; }
}