using CMS.Contracts.Admin.Dashboard;

namespace CMS.Server.Services.Statistics;

public interface IStatQueryService
{
    Task<DashboardStatsDto> GetDashboardStatsAsync(CancellationToken cancellationToken = default);
}