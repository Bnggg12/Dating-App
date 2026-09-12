using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Server.Core.Exceptions;
using Server.Data;
using Server.DTOs;
using Server.Mappers;
using Server.Models;

namespace Server.Services;

public class AdminService(UserManager<AppUser> userManager, AppDbContext db, PhotoService photoService)
{
    public async Task<IEnumerable<UserRoleDto>> GetUserWithRolesAsync()
    {
        var users = await userManager.Users
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .Include(u => u.Photos)
            .OrderBy(u => u.Id)
            .ToListAsync();

        return users.Select(u => u.ToApproveDto());
    }

    public async Task<IList<string>> EditRolesAsync(int userId, string roles)
    {
        if (string.IsNullOrEmpty(roles))
            throw new BadRequestException("Phải chọn ít nhất một vai trò");

        var selectedRoles = roles.Split(",").Select(r => r.Trim()).ToArray();

        var user = await userManager.FindByIdAsync(userId.ToString())
            ?? throw new NotFoundException("Không tìm thấy người dùng");

        var userRoles = await userManager.GetRolesAsync(user);

        var addResult = await userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));
        if (!addResult.Succeeded) throw new BadRequestException("Thêm vai trò thất bại");

        var removeResult = await userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));
        if (!removeResult.Succeeded) throw new BadRequestException("Gỡ vai trò thất bại");

        return await userManager.GetRolesAsync(user);
    }

    public async Task<IEnumerable<ApprovePhotoDto>> GetPhotosForApproveAsync()
    {
        var photos = await db.Photos
            .Include(p => p.User)
            .Where(p => !p.IsApproved)
            .ToListAsync();

        return photos.Select(p => p.ToApproveDto());
    }

    public async Task ApprovePhotoAsync(int photoId)
    {
        var photo = await db.Photos.FindAsync(photoId)
            ?? throw new NotFoundException("Không tìm thấy ảnh");

        photo.IsApproved = true;

        if (await db.Photos.AnyAsync(p => p.UserId == photo.UserId && p.IsMain && p.IsApproved) == false)
            photo.IsMain = true;

        await db.SaveChangesAsync();
    }

    public async Task RejectPhotoAsync(int photoId)
    {
        var photo = await db.Photos.FindAsync(photoId)
            ?? throw new NotFoundException("Không tìm thấy ảnh");

        if (!string.IsNullOrEmpty(photo.PublicId))
        {
            var deleteResult = await photoService.DeletePhotoCloudinaryAsync(photo.PublicId);
            if (deleteResult.Error != null) throw new BadRequestException(deleteResult.Error.Message);
        }

        db.Photos.Remove(photo);
        await db.SaveChangesAsync();
    }
}