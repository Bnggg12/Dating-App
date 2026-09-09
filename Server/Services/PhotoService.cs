using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Server.Core.Exceptions;
using Server.Data;
using Server.DTOs;
using Server.Helpers;
using Server.Mappers;
using Server.Models;

namespace Server.Services;

public class PhotoService
{
    private readonly Cloudinary _cloudinary;
    private readonly AppDbContext _db;
    public PhotoService(IOptions<CloudinarySettings> config, AppDbContext db)
    {
        var account = new Account(config.Value.CloudName, config.Value.ApiKey, config.Value.ApiSecret);
        _cloudinary = new Cloudinary(account);
        _db = db;
    }

    public async Task<PhotoDto> UploadPhotoAsync(IFormFile file, int userId)
    {
        var user = await _db.Users
            .Include(u => u.Photos)
            .FirstOrDefaultAsync(u => u.Id == userId)
            ?? throw new NotFoundException("User not found");

        var uploadResult = await UploadPhotoCloudinaryAsync(file);
        if (uploadResult.Error != null) throw new BadRequestException(uploadResult.Error.Message);

        var photo = PhotoMapper.ToEntity(uploadResult, userId, user.Photos.Count == 0);

        user.Photos.Add(photo);

        if (await _db.SaveChangesAsync() <= 0)
            throw new BadRequestException("Error saving images");

        return PhotoMapper.ToDto(photo);
    }

    public async Task SetMainPhotoAsync(int photoId, int userId)
    {
        var user = await _db.Users
            .Include(u => u.Photos)
            .FirstOrDefaultAsync(u => u.Id == userId)
            ?? throw new NotFoundException("User not found");

        var photo = user.Photos.FirstOrDefault(p => p.Id == photoId)
            ?? throw new NotFoundException("Image not found");

        if (photo.IsMain)
            throw new BadRequestException("This photo is already the main profile image");

        var currentMain = user.Photos.FirstOrDefault(p => p.IsMain);
        currentMain?.IsMain = false;

        photo.IsMain = true;

        if (await _db.SaveChangesAsync() <= 0)
            throw new BadRequestException("Unable to update profile image");
    }

    public async Task DeletePhotoAsync(int photoId, int userId)
    {
        var user = await _db.Users
            .Include(u => u.Photos)
            .FirstOrDefaultAsync(u => u.Id == userId)
            ?? throw new NotFoundException("User not found");

        var photo = user.Photos.FirstOrDefault(p => p.Id == photoId)
            ?? throw new NotFoundException("Image not found");

        if (photo.IsMain)
            throw new BadRequestException("Cannot delete the main profile image");

        if (!string.IsNullOrEmpty(photo.PublicId))
        {
            var result = await DeletePhotoCloudinaryAsync(photo.PublicId);
            if (result.Error != null)
                throw new BadRequestException(result.Error.Message);
        }

        user.Photos.Remove(photo);

        if (await _db.SaveChangesAsync() <= 0)
            throw new BadRequestException("Unable to delete profile image");
    }

    private async Task<ImageUploadResult> UploadPhotoCloudinaryAsync(IFormFile file)
    {
        var uploadResult = new ImageUploadResult();

        if (file.Length > 0)
        {
            await using var stream = file.OpenReadStream();
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Transformation = new Transformation().Height(500).Width(500).Crop("fill").Gravity("face"),
                Folder = "dating-app"
            };
            uploadResult = await _cloudinary.UploadAsync(uploadParams);
        }

        return uploadResult;
    }

    private async Task<DeletionResult> DeletePhotoCloudinaryAsync(string publicId)
    {
        var deleteParams = new DeletionParams(publicId);
        return await _cloudinary.DestroyAsync(deleteParams);
    }
}