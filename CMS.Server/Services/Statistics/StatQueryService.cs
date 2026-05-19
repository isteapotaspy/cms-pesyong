using CMS.Contracts.Admin.Dashboard;
using CMS.Domain.Enums;
using CMS.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace CMS.Server.Services.Statistics;

public sealed class StatQueryService : IStatQueryService
{
    private readonly CmsDbContext _db;

    public StatQueryService(CmsDbContext db)
    {
        _db = db;
    }

    public async Task<DashboardStatsDto> GetDashboardStatsAsync(CancellationToken cancellationToken = default)
    {
        var totalOrders = await _db.Orders
            .CountAsync(cancellationToken);

        var pendingOrders = await _db.Orders
            .Where(x => x.Status == OrderStatus.Pending)
            .CountAsync(cancellationToken);

        var confirmedOrders = await _db.Orders
            .Where(x => x.Status == OrderStatus.Confirmed)
            .CountAsync(cancellationToken);

        var deliveredOrders = await _db.Orders
            .Where(x => x.Status == OrderStatus.Delivered)
            .CountAsync(cancellationToken);

        var totalCustomers = await _db.CustomerProfiles
            .CountAsync(cancellationToken);

        var totalPackages = await _db.Packages
            .Where(x => x.IsAvailable)
            .CountAsync(cancellationToken);

        var totalMeals = await _db.Meals
            .Where(x => x.IsAvailable)
            .CountAsync(cancellationToken);

        var totalRevenue = await _db.Orders
            .Where(x => x.Status != OrderStatus.Cancelled)
            .SumAsync(x => (decimal?)x.GrandTotal, cancellationToken) ?? 0m;

        var averageOrderValue = totalOrders > 0
            ? Math.Round(totalRevenue / totalOrders, 2)
            : 0m;

        return new DashboardStatsDto
        {
            TotalOrders = totalOrders,
            PendingOrders = pendingOrders,
            ConfirmedOrders = confirmedOrders,
            DeliveredOrders = deliveredOrders,

            TotalPackages = totalPackages,
            TotalMeals = totalMeals,

            TotalRevenue = totalRevenue,
            AverageOrderValue = averageOrderValue
        };
    }
}