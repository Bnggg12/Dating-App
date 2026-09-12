using Microsoft.EntityFrameworkCore;
using Server.DTOs;
using Server.Models;
using Server.Mappers;
using Server.Core.Paginations;
using Server.Data;
using Server.Core.Exceptions;

namespace Server.Services;

public class LikeService(AppDbContext db)
{
    public async Task<UserLike?> GetUserLike(int SourceMemberId, int TargetMemberId)
    {
        return await db.Likes.FindAsync(SourceMemberId, TargetMemberId);
    }

    public async Task ToggleLike(int SourceMemberId, int TargetMemberId)
    {
        if (SourceMemberId == TargetMemberId)
            throw new BadRequestException("Bạn không thể tự kết nối chính mình");

        var targetUser = await db.Users.FindAsync(TargetMemberId) 
            ?? throw new NotFoundException("Không tìm thấy người dùng");

        var existingLike = await GetUserLike(SourceMemberId, TargetMemberId);

        if (existingLike == null)
        {
            var userLike = new UserLike
            {
                SourceMemberId = SourceMemberId,
                TargetMemberId = TargetMemberId
            };
            db.Likes.Add(userLike);
        }
        else db.Likes.Remove(existingLike);

        var success = await db.SaveChangesAsync() > 0;
        if (!success) throw new BadRequestException("Không thể thực hiện thao tác");
    }

    public async Task<IEnumerable<int>> GetCurrentUserLikeIds(int currentUserId)
    {
        return await db.Likes
            .Where(x => x.SourceMemberId == currentUserId)
            .Select(x => x.TargetMemberId)
            .ToListAsync();
    }

    public async Task<PaginatedResult<UserLikeDto>> GetUserLikes(LikesParams likesParams)
    {
        var likes = db.Likes.AsQueryable();
        IQueryable<AppUser> query;

        switch (likesParams.Type)
        {
            case "liked":
                query = likes
                    .Where(x => x.SourceMemberId == likesParams.UserId)
                    .Include(x => x.TargetMember)
                        .ThenInclude(u => u.Photos)
                    .Select(x => x.TargetMember);
                break;

            case "likedBy":
                query = likes
                    .Where(x => x.TargetMemberId == likesParams.UserId)
                    .Include(x => x.SourceMember)
                        .ThenInclude(u => u.Photos)
                    .Select(x => x.SourceMember);
                break;

            default:
                var likeIds = await GetCurrentUserLikeIds(likesParams.UserId);

                query = likes
                    .Where(x => x.TargetMemberId == likesParams.UserId && likeIds.Contains(x.SourceMemberId))
                    .Include(x => x.SourceMember)
                        .ThenInclude(u => u.Photos)
                    .Select(x => x.SourceMember);
                break;
        }

        var count = await query.CountAsync();
        var users = await query
            .Skip((likesParams.PageNumber - 1) * likesParams.PageSize)
            .Take(likesParams.PageSize)
            .ToListAsync();

        var items = users.Select(LikeMapper.ToDto).ToList();

        return new PaginatedResult<UserLikeDto>(items, count, likesParams.PageNumber, likesParams.PageSize);
    }
}