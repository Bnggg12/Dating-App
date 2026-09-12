using Microsoft.EntityFrameworkCore;
using Server.Core.Exceptions;
using Server.Core.Paginations;
using Server.Data;
using Server.DTOs;
using Server.Mappers;
using Server.Models;

namespace Server.Services;

public class UserService(AppDbContext db)
{
    public async Task<PaginatedResult<UserCardDto>> GetAsync(UserParams userParams)
    {
        var query = db.Users
            .AsNoTracking()
            .Where(u => u.Id != userParams.UserId)
            .Include(u => u.Photos)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(userParams.Search))
        {
            var search = userParams.Search.Trim().ToLower();
            query = query.Where(u => u.DisplayName.ToLower().Contains(search));
        }

        if (!string.IsNullOrWhiteSpace(userParams.Gender))
            query = query.Where(u => u.Gender == userParams.Gender);

        if (!string.IsNullOrWhiteSpace(userParams.LookingFor))
            query = query.Where(u => u.LookingFor == userParams.LookingFor);

        if (!string.IsNullOrWhiteSpace(userParams.City))
            query = query.Where(u => u.City == userParams.City);

        if (!string.IsNullOrWhiteSpace(userParams.Mbti))
            query = query.Where(u => u.Mbti == userParams.Mbti);

        if (!string.IsNullOrWhiteSpace(userParams.EducationLevel))
            query = query.Where(u => u.EducationLevel == userParams.EducationLevel);

        var today = DateOnly.FromDateTime(DateTime.Today);
        var minDob = today.AddYears(-userParams.MaxAge - 1);
        var maxDob = today.AddYears(-userParams.MinAge);

        query = query.Where(u => u.DateOfBirth > minDob && u.DateOfBirth <= maxDob);

        query = userParams.OrderBy switch
        {
            "created" => query.OrderByDescending(u => u.CreatedAt),
            _ => query.OrderByDescending(u => u.LastActive)
        };

        var count = await query.CountAsync();
        var users = await query
            .Skip((userParams.PageNumber - 1) * userParams.PageSize)
            .Take(userParams.PageSize)
            .ToListAsync();

        var items = users.Select(u => u.ToCardDto()).ToList();

        return new PaginatedResult<UserCardDto>(items, count, userParams.PageNumber, userParams.PageSize);
    }

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