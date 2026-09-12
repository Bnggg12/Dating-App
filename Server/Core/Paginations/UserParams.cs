namespace Server.Core.Paginations;

public class UserParams : PagingParams
{
    public int UserId { get; set; }

    public string? Search { get; set; }

    public string? Gender { get; set; }
    public string? LookingFor { get; set; }
    public string? City { get; set; }
    public string? Mbti { get; set; }
    public string? EducationLevel { get; set; }

    public int MinAge { get; set; } = 18;
    public int MaxAge { get; set; } = 100;

    public string OrderBy { get; set; } = "lastActive";
}