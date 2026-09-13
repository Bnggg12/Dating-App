using Server.DTOs;
using Server.Helpers;
using Server.Models;

namespace Server.Mappers;

public static class MessageMapper
{
    public static MessageDto ToDto(this Message message)
    {
        return new MessageDto
        {
            Id = message.Id,
            SenderId = message.SenderId,
            SenderDisplayName = message.Sender.DisplayName,
            SenderImageUrl = message.Sender.Photos.FirstOrDefault(x => x.IsMain && x.IsApproved)?.Url,

            RecipientId = message.RecipientId,
            RecipientDisplayName = message.Recipient.DisplayName,
            RecipientImageUrl = message.Recipient.Photos.FirstOrDefault(x => x.IsMain && x.IsApproved)?.Url,

            Content = message.Content,
            MessageSent = message.MessageSent,
            DateRead = message.DateRead
        };
    }

    public static Message ToEntity(this MessageCreateDto dto, int senderId)
    {
        return new Message
        {
            SenderId = senderId,
            RecipientId = dto.RecipientId,
            Content = dto.Content.Trim(),
            MessageSent = TimeHelper.NowVN()
        };
    }
}