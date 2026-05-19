using CMS.Contracts.Admin.Dashboard;

namespace PESYONG.Presentation.Admin.Interfaces;

public interface IStatApiService : IAsyncDisposable
{
    event Action<DashboardStatsDto>? StatsUpdated;

    bool IsConnected { get; }

    Task<DashboardStatsDto?> GetDashboardStatsAsync(CancellationToken cancellationToken = default);

    Task StartStatsStreamAsync(CancellationToken cancellationToken = default);

    Task StopStatsStreamAsync(CancellationToken cancellationToken = default);
}