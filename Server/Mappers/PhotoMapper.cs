using Server.DTOs;
using Server.Models;

namespace Server.Mappers;

public static class PhotoMapper
{
    public static PhotoDto ToDto(this Photo photo)
    {
        return new PhotoDto
        {
            Id = photo.Id,
            Url = photo.Url,
            IsMain = photo.IsMain,
            IsApproved = photo.IsApproved
        };
    }
}