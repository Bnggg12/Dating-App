using Server.DTOs;
using Server.Helpers;
using Server.Models;
using Server.Services;

namespace Server.Mappers;

public static class UserMapper
{
    public static UserDto ToDto(this AppUser user, TokenService tokenService, List<string> roles)
    {
        return new UserDto
        {
            Id = user.Id,
            Email = user.Email!,
            DisplayName = user.DisplayName,
            ImageUrl = user.Photos.FirstOrDefault(p => p.IsMain && p.IsApproved)?.Url,
            Token = tokenService.CreateToken(user),
            Roles = roles
        };
    }
    
    public static AppUser ToEntity(this RegisterDto dto)
    {
        return new AppUser
        {
            UserName = dto.Email,
            Email = dto.Email,
            DisplayName = dto.DisplayName,
            DateOfBirth = dto.DateOfBirth,
            Gender = dto.Gender,
            Description = dto.Description,
            City = dto.City,
            CreatedAt = TimeHelper.NowVN(),
            LastActive = TimeHelper.NowVN()
        };
    }

    public static void ToDto(this AppUser user, UserUpdateDto dto)
    {
        user.DisplayName = dto.DisplayName;
        user.Description = dto.Description;
        user.City = dto.City;
        user.LookingFor = dto.LookingFor;
        user.Mbti = dto.Mbti;
        user.EducationLevel = dto.EducationLevel;
        user.FieldOfStudy = dto.FieldOfStudy;
        user.Institution = dto.Institution;
        user.Interests = dto.Interests;
    }
}