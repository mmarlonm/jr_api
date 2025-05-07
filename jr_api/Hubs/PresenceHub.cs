using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

public class PresenceHub : Hub
{
    private static readonly ConcurrentDictionary<string, UserConnection> _connections = new();

    public override async Task OnConnectedAsync()
    {
        var httpContext = Context.GetHttpContext();
        var userId = httpContext.Request.Query["usuarioId"];

        _connections[Context.ConnectionId] = new UserConnection
        {
            ConnectionId = Context.ConnectionId,
            UserId = userId,
            LastHeartbeat = DateTime.UtcNow
        };

        await Clients.All.SendAsync("UserConnected", userId); // 🚀 Notifica que un usuario se conectó
        await Clients.All.SendAsync("ConnectedUsersUpdated", GetConnectedUserIds());
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        if (_connections.TryRemove(Context.ConnectionId, out var removed))
        {
            await Clients.All.SendAsync("UserDisconnected", removed.UserId); // 🚪 Notifica que se desconectó
            await Clients.All.SendAsync("ConnectedUsersUpdated", GetConnectedUserIds());
        }

        await base.OnDisconnectedAsync(exception);
    }

    public Task Heartbeat()
    {
        if (_connections.TryGetValue(Context.ConnectionId, out var user))
        {
            user.LastHeartbeat = DateTime.UtcNow;
        }
        return Task.CompletedTask;
    }

    public static List<string> GetConnectedUserIds()
    {
        return _connections.Values.Select(u => u.UserId).Distinct().ToList();
    }

    public static void DisconnectInactiveUsers(TimeSpan timeout)
    {
        var now = DateTime.UtcNow;
        foreach (var conn in _connections.Values.ToList())
        {
            if (now - conn.LastHeartbeat > timeout)
            {
                _connections.TryRemove(conn.ConnectionId, out _);
            }
        }
    }
}