using System.Collections.Concurrent;

namespace Server.Core.Hubs;

public class PresenceTracker
{
    private static readonly ConcurrentDictionary<int, ConcurrentDictionary<string, byte>> OnlineUsers = new();

    public Task<bool> UserConnected(int userId, string connectionId)
    {
        var isOnline = false;
        var connection = OnlineUsers.GetOrAdd(userId, _ =>
        {
            isOnline = true;
            return new ConcurrentDictionary<string, byte>();
        });
        connection.TryAdd(connectionId, 0);
        return Task.FromResult(isOnline);
    }

    public Task<bool> UserDisconnected(int userId, string connectionId)
    {
        var isOffline = false;
        if (OnlineUsers.TryGetValue(userId, out var connections))
        {
            connections.TryRemove(connectionId, out _);
            if (connections.IsEmpty)
            {
                OnlineUsers.TryRemove(userId, out _);
                isOffline = true;
            }
        }

        return Task.FromResult(isOffline);
    }

    public Task<int[]> GetOnlineUsers()
    {
        return Task.FromResult(OnlineUsers.Keys.OrderBy(k => k).ToArray());
    }

    public Task<List<string>> GetConnectionsForUser(int userId)
    {
        if (OnlineUsers.TryGetValue(userId, out var connections))
            return Task.FromResult(connections.Keys.ToList());

        return Task.FromResult(new List<string>());
    }
}