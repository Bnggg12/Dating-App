using Microsoft.EntityFrameworkCore;
using Server.Core.Exceptions;
using Server.Core.Paginations;
using Server.Data;
using Server.DTOs;
using Server.Helpers;
using Server.Mappers;

namespace Server.Services;

public class MessageService(AppDbContext db)
{
    public async Task<PaginatedResult<MessageDto>> GetMessagesAsync(MessageParams messageParams)
    {
        var query = db.Messages
            .Include(m => m.Sender).ThenInclude(u => u.Photos)
            .Include(m => m.Recipient).ThenInclude(u => u.Photos)
            .AsNoTracking()
            .AsQueryable();

        query = messageParams.Container.ToLower() switch
        {
            "outbox" => query.Where(m => m.SenderId == messageParams.UserId && !m.SenderDeleted),
            _ => query.Where(m => m.RecipientId == messageParams.UserId && !m.RecipientDeleted)
        };

        query = query.OrderByDescending(m => m.MessageSent);

        var count = await query.CountAsync();
        var messages = await query
            .Skip((messageParams.PageNumber - 1) * messageParams.PageSize)
            .Take(messageParams.PageSize)
            .ToListAsync();

        var dtos = messages.Select(m => m.ToDto()).ToList();

        return new PaginatedResult<MessageDto>(dtos, count, messageParams.PageNumber, messageParams.PageSize);
    }

    public async Task<IReadOnlyList<MessageDto>> GetMessageThreadAsync(int currentUserId, int recipientId)
    {
        var messages = await db.Messages
            .Include(m => m.Sender).ThenInclude(u => u.Photos)
            .Include(m => m.Recipient).ThenInclude(u => u.Photos)
            .Where(m => 
                (m.RecipientId == currentUserId && !m.RecipientDeleted && m.SenderId == recipientId) ||
                (m.SenderId == currentUserId && !m.SenderDeleted && m.RecipientId == recipientId))
            .OrderBy(m => m.MessageSent)
            .ToListAsync();

        var unreadMessages = messages
            .Where(m => m.DateRead == null && m.RecipientId == currentUserId)
            .ToList();

        if (unreadMessages.Count != 0)
        {
            foreach (var message in unreadMessages) message.DateRead = TimeHelper.NowVN();
            await db.SaveChangesAsync();
        }

        return messages.Select(m => m.ToDto()).ToList();
    }

    public async Task<MessageDto> AddMessageAsync(int senderId, MessageCreateDto dto)
    {
        if (senderId == dto.RecipientId)
            throw new BadRequestException("Bạn không thể gửi tin nhắn cho chính mình");

        var recipientExists = await db.Users.AnyAsync(u => u.Id == dto.RecipientId);
        if (!recipientExists) throw new NotFoundException("Không tìm thấy người nhận");

        var message = dto.ToEntity(senderId);
        db.Messages.Add(message);
        await db.SaveChangesAsync();

        await db.Entry(message)
            .Reference(m => m.Sender)
            .Query()
            .Include(u => u.Photos)
            .LoadAsync();

        await db.Entry(message)
            .Reference(m => m.Recipient)
            .Query()
            .Include(u => u.Photos)
            .LoadAsync();

        return message.ToDto();
    }

    public async Task DeleteMessageAsync(int currentUserId, int messageId)
    {
        var message = await db.Messages.FindAsync(messageId)
            ?? throw new NotFoundException("Không tìm thấy tin nhắn");

        if (message.SenderId != currentUserId && message.RecipientId != currentUserId)
            throw new UnauthorizedException("Bạn không có quyền xóa tin nhắn này");

        if (message.SenderId == currentUserId) message.SenderDeleted = true;
        if (message.RecipientId == currentUserId) message.RecipientDeleted = true;

        if (message.SenderDeleted && message.RecipientDeleted) db.Messages.Remove(message);

        await db.SaveChangesAsync();
    }
}