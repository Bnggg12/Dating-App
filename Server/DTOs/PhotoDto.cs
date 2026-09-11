namespace Server.DTOs;

public class PhotoDto
{
    public int Id { get; set; }
    public string Url { get; set; } = null!;
    public bool IsMain { get; set; }
    public bool IsApproved { get; set; }
}

public class ApprovePhotoDto
{
    public int Id { get; set; }
    public string Url { get; set; } = null!;
    public int UserId { get; set; }
    public string DisplayName { get; set; } = null!;
}