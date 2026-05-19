namespace CMS.Server.Services.Statistics;

public interface IStatBroadcaster
{
    Task BroadcastDashboardStatsAsync(CancellationToken cancellationToken = default);
}