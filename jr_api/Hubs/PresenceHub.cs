using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

public class PresenceHub : Hub
{
    private static readonly ConcurrentDictionary<string, UserConnection> _connections = new();
    public static IReadOnlyDictionary<string, UserConnection> Connections => _connections;

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

        await Clients.All.SendAsync("UserConnected", userId);
        await Clients.All.SendAsync("ConnectedUsersUpdated", GetConnectedUsersWithStatus());

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        if (_connections.TryRemove(Context.ConnectionId, out var removed))
        {
            await Clients.All.SendAsync("UserDisconnected", removed.UserId);
            await Clients.All.SendAsync("ConnectedUsersUpdated", GetConnectedUsersWithStatus());
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

    public async Task SetAway(string usuarioId)
    {
        Console.WriteLine($"⏳ Usuario inactivo: {usuarioId}");
        await Clients.All.SendAsync("UserSetAway", usuarioId);
        await Clients.All.SendAsync("ConnectedUsersUpdated", GetConnectedUsersWithStatus());
    }

    public async Task SetActive(string usuarioId)
    {
        Console.WriteLine($"✅ Usuario activo: {usuarioId}");
        await Clients.All.SendAsync("UserSetActive", usuarioId);
        await Clients.All.SendAsync("ConnectedUsersUpdated", GetConnectedUsersWithStatus());
    }

    public static List<UsuarioEstadoDto> GetConnectedUsersWithStatus()
    {
        var now = DateTime.UtcNow;
        var timeout = TimeSpan.FromMinutes(1);

        return _connections.Values
            .GroupBy(u => u.UserId)
            .Select(group =>
            {
                var conexiones = group.ToList();

                var activo = conexiones.Any(c => now - c.LastHeartbeat <= timeout);
                return new UsuarioEstadoDto
                {
                    UserId = group.Key,
                    Status = activo ? "Activo" : "Inactivo"
                };
            })
            .ToList();
    }

    public static void DisconnectInactiveUsers(TimeSpan timeout)
    {
        var now = DateTime.UtcNow;
        foreach (var conn in _connections.Values.ToList())
        {
            if (now - conn.LastHeartbeat > timeout)
            {
                _connections.TryRemove(conn.ConnectionId, out var removed);
                if (removed != null)
                {
                    Console.WriteLine($"🔌 Usuario desconectado por inactividad: {removed.UserId}");
                }
            }
        }
    }

    public static bool TryRemoveConnection(string connectionId, out UserConnection? user)
    {
        return _connections.TryRemove(connectionId, out user!);
    }
}