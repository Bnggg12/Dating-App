using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Server.Core.Exceptions;
using Server.Core.SignalR;
using Server.DTOs;
using Server.Services;

namespace Server.Core.Hubs;

[Authorize]
public class MessageHub(MessageService messageService, IHubContext<PresenceHub> presenceHub, PresenceTracker presenceTracker) : Hub
{
    private int GetUserId() => int.Parse(Context.User!.FindFirstValue(ClaimTypes.NameIdentifier)!);

    public override async Task OnConnectedAsync()
    {
        var httpContext = Context.GetHttpContext();
        var otherUser = httpContext?.Request.Query["userId"].ToString();

        if (string.IsNullOrEmpty(otherUser) || !int.TryParse(otherUser, out var otherUserId))
            throw new HubException("Không tìm thấy thông tin đối phương");

        var groupName = GetGroupName(GetUserId(), otherUserId);

        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

        var messages = await messageService.GetMessageThreadAsync(GetUserId(), otherUserId);

        await Clients.Group(groupName).SendAsync("MessagesMarkedRead", GetUserId());
        await Clients.Caller.SendAsync("ReceiveMessageThread", messages);
    }

    public async Task SendMessage(MessageCreateDto dto)
    {
        var message = await messageService.AddMessageAsync(GetUserId(), dto);
        var groupName = GetGroupName(GetUserId(), dto.RecipientId);

        await Clients.Group(groupName).SendAsync("NewMessage", message);

        var recipientConnections = await presenceTracker.GetConnectionsForUser(dto.RecipientId);
        if (recipientConnections.Count > 0)
            await presenceHub.Clients.Clients(recipientConnections).SendAsync("NewMessageReceived", message);
    }

    public async Task UpdateRead(int otherUserId)
    {
        var groupName = GetGroupName(GetUserId(), otherUserId);
        await Clients.Group(groupName).SendAsync("MessagesMarkedRead", GetUserId());
    }

    private static string GetGroupName(int caller, int other)
    {
        return caller < other ? $"{caller}-{other}" : $"{other}-{caller}";
    }
}