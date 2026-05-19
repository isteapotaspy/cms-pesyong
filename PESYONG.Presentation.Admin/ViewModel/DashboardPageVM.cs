using System.Windows;
using CMS.Contracts.Admin.Dashboard;
using CommunityToolkit.Mvvm.ComponentModel;
using PESYONG.Presentation.Admin.Interfaces;

namespace PESYONG.Presentation.Admin.ViewModels;

public partial class DashboardPageVM : ObservableObject, IAsyncDisposable
{
    private readonly IStatApiService _statApiService;

    [ObservableProperty]
    private int totalOrders;

    [ObservableProperty]
    private int pendingOrders;

    [ObservableProperty]
    private int confirmedOrders;

    [ObservableProperty]
    private int deliveredOrders;

    [ObservableProperty]
    private int totalCustomers;

    [ObservableProperty]
    private int totalPackages;

    [ObservableProperty]
    private int totalMeals;

    [ObservableProperty]
    private decimal totalRevenue;

    [ObservableProperty]
    private decimal averageOrderValue;

    [ObservableProperty]
    private bool isStatsConnected;

    public DashboardPageVM(IStatApiService statApiService)
    {
        _statApiService = statApiService;
    }

    public async Task LoadAsync()
    {
        var initialStats = await _statApiService.GetDashboardStatsAsync();

        if (initialStats is not null)
            ApplyStats(initialStats);

        _statApiService.StatsUpdated += OnStatsUpdated;

        await _statApiService.StartStatsStreamAsync();

        IsStatsConnected = _statApiService.IsConnected;
    }

    private void OnStatsUpdated(DashboardStatsDto stats)
    {
        Application.Current.Dispatcher.Invoke(() =>
        {
            ApplyStats(stats);
            IsStatsConnected = _statApiService.IsConnected;
        });
    }

    private void ApplyStats(DashboardStatsDto stats)
    {
        TotalOrders = stats.TotalOrders;
        PendingOrders = stats.PendingOrders;
        ConfirmedOrders = stats.ConfirmedOrders;
        DeliveredOrders = stats.DeliveredOrders;

        TotalPackages = stats.TotalPackages;
        TotalMeals = stats.TotalMeals;

        TotalRevenue = stats.TotalRevenue;
        AverageOrderValue = stats.AverageOrderValue;
    }

    public async ValueTask DisposeAsync()
    {
        _statApiService.StatsUpdated -= OnStatsUpdated;
        await _statApiService.StopStatsStreamAsync();
    }
}