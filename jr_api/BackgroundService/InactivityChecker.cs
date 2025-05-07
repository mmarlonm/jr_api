public class InactivityChecker : BackgroundService
{
    private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(1);
    private readonly TimeSpan _inactivityLimit = TimeSpan.FromMinutes(5);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            PresenceHub.DisconnectInactiveUsers(_inactivityLimit);
            await Task.Delay(_checkInterval, stoppingToken);
        }
    }
}