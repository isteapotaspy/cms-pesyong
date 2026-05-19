using CMS.Contracts.Admin.Dashboard;
using CMS.Server.Services.Statistics;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace CMS.Server.Hubs;

public sealed class DashboardStatsHub : Hub
{
    private readonly IStatQueryService _statQueryService;

    public DashboardStatsHub(IStatQueryService statQueryService)
    {
        _statQueryService = statQueryService;
    }

    public override async Task OnConnectedAsync()
    {
        var currentStats = await _statQueryService.GetDashboardStatsAsync(Context.ConnectionAborted);

        await Clients.Caller.SendAsync(
            DashboardStatsEvents.StatsUpdated,
            currentStats,
            Context.ConnectionAborted);

        await base.OnConnectedAsync();
    }

    public async Task<DashboardStatsDto> GetCurrentStats()
    {
        return await _statQueryService.GetDashboardStatsAsync(Context.ConnectionAborted);
    }
}