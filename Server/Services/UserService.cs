using Microsoft.EntityFrameworkCore;
using Server.Core.Exceptions;
using Server.Data;
using Server.DTOs;
using Server.Mappers;
using Server.Models;

namespace Server.Services;

public class UserService(AppDbContext db)
{
    public async Task<UserProfileDto> GetByIdAsync(int id)
    {
        var user = await db.Users
            .Include(u => u.Photos)
            .FirstOrDefaultAsync(u => u.Id == id)
            ?? throw new NotFoundException("User not found");

        return UserMapper.ToDto(user);
    }

    public async Task UpdateAsync(int userId, UserUpdateDto dto)
    {
        var user = await db.Users.FindAsync(userId)
            ?? throw new NotFoundException("User not found");

        UserMapper.ToDto(user, dto);

        if (db.ChangeTracker.HasChanges())
        {
            if (await db.SaveChangesAsync() <= 0)
                throw new BadRequestException("Unable to update user information");
        }
    }
}