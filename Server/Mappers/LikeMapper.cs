using Server.DTOs;
using Server.Models;

namespace Server.Mappers;

public static class LikeMapper
{
    public static UserLikeDto ToDto(this AppUser entity)
    {
        return new UserLikeDto
        {
            Id = entity.Id,
            DisplayName = entity.DisplayName,
            DateOfBirth = entity.DateOfBirth,
            City = entity.City,
            ImageUrl = entity.Photos.FirstOrDefault(p => p.IsMain && p.IsApproved)?.Url
        };
    }
}