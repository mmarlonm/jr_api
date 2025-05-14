using Microsoft.AspNetCore.SignalR;

public class InactivityChecker : BackgroundService
{
    private readonly IHubContext<PresenceHub> _hubContext;
    private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(1);
    private readonly TimeSpan _inactivityLimit = TimeSpan.FromMinutes(5);

    public InactivityChecker(IHubContext<PresenceHub> hubContext)
    {
        _hubContext = hubContext;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var now = DateTime.UtcNow;

            foreach (var kvp in PresenceHub.Connections)
            {
                var conn = kvp.Value;
                if (now - conn.LastHeartbeat > _inactivityLimit)
                {
                    if (PresenceHub.TryRemoveConnection(conn.ConnectionId, out var removed))
                    {
                        Console.WriteLine("usuario a desconectar ", removed.UserId);
                        await _hubContext.Clients.All.SendAsync("UserDisconnected", removed.UserId);
                        await _hubContext.Clients.All.SendAsync("ConnectedUsersUpdated",
                            PresenceHub.Connections.Values.Select(u => u.UserId).Distinct().ToList());
                        await _hubContext.Clients.Client(conn.ConnectionId)
                            .SendAsync("ForceDisconnect"); // opcional: señal para que cliente se desconecte
                    }
                }
            }

            await Task.Delay(_checkInterval, stoppingToken);
        }
    }
}