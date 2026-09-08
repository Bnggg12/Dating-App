using System.ComponentModel.DataAnnotations;

namespace Server.DTOs;

public class UserDto
{
    public int Id { get; set; }
    public string Email { get; set; } = null!;
    public string DisplayName { get; set; } = null!;
    public string? ImageUrl { get; set; }
    public string Token { get; set; } = null!;
    public List<string> Roles { get; set; } = [];
}

public class LoginDto
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
}

public class RegisterDto
{
    public string Email { get; set; } = null!;
    [MinLength(8)] 
    public string Password { get; set; } = null!;
    public string DisplayName { get; set; } = null!;
    public DateOnly DateOfBirth { get; set; }
    public string Gender { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string City { get; set; } = null!;
}

public class UserUpdateDto
{
    public string DisplayName { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string City { get; set; } = null!;
    public string LookingFor { get; set; } = null!;

    public string? Mbti { get; set; }
    public string? EducationLevel { get; set; }
    public string? FieldOfStudy { get; set; }
    public string? Institution { get; set; }
    public List<string> Interests { get; set; } = [];
}

public class UserProfileDto
{
    public int Id { get; set; }
    public string Email { get; set; } = null!;
    public string DisplayName { get; set; } = null!;
    public int Age { get; set; }
    public string Gender { get; set; } = null!;
    public string LookingFor { get; set; } = null!;
    public string Description { get; set; } = null!;
    public string City { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime LastActive { get; set; }

    public string? Mbti { get; set; }
    public string? EducationLevel { get; set; }
    public string? FieldOfStudy { get; set; }
    public string? Institution { get; set; }
    public List<string> Interests { get; set; } = [];

    public List<PhotoDto> Photos { get; set; } = [];
}