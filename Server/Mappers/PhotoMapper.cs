using CloudinaryDotNet.Actions;
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

    public static Photo ToEntity(this ImageUploadResult uploadResult, int userId, bool isMain)
    {
        return new Photo
        {
            Url = uploadResult.SecureUrl.AbsoluteUri,
            PublicId = uploadResult.PublicId,
            IsApproved = false,
            IsMain = isMain,
            UserId = userId
        };
    }
}