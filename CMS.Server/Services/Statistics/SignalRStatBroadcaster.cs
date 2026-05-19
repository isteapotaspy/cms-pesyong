using CMS.Contracts.Admin.Dashboard;
using CMS.Server.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace CMS.Server.Services.Statistics;

public sealed class SignalRStatBroadcaster : IStatBroadcaster
{
    private readonly IStatQueryService _statQueryService;
    private readonly IHubContext<DashboardStatsHub> _hubContext;

    public SignalRStatBroadcaster(
        IStatQueryService statQueryService,
        IHubContext<DashboardStatsHub> hubContext)
    {
        _statQueryService = statQueryService;
        _hubContext = hubContext;
    }

    public async Task BroadcastDashboardStatsAsync(CancellationToken cancellationToken = default)
    {
        var stats = await _statQueryService.GetDashboardStatsAsync(cancellationToken);

        await _hubContext.Clients.All.SendAsync(
            DashboardStatsEvents.StatsUpdated,
            stats,
            cancellationToken);
    }
}