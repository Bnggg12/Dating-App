using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Server.Core.Hubs;

namespace Server.Core.SignalR;

[Authorize]
public class PresenceHub(PresenceTracker presenceTracker) : Hub
{
    private int GetUserId() => int.Parse(Context.User!.FindFirstValue(ClaimTypes.NameIdentifier)!);

    public override async Task OnConnectedAsync()
    {
        var isOnline = await presenceTracker.UserConnected(GetUserId(), Context.ConnectionId);
        if (isOnline) await Clients.Others.SendAsync("UserOnline", GetUserId());

        var currentUsers = await presenceTracker.GetOnlineUsers();
        await Clients.Caller.SendAsync("GetOnlineUsers", currentUsers);
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var isOffline = await presenceTracker.UserDisconnected(GetUserId(), Context.ConnectionId);
        if (isOffline) await Clients.Others.SendAsync("UserOffline", GetUserId());

        await base.OnDisconnectedAsync(exception);
    }
}