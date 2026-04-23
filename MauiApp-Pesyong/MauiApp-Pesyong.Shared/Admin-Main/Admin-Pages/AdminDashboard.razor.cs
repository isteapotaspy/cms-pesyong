using Microsoft.AspNetCore.Components;
using MauiApp_Pesyong.Shared.Admin_Main.Admin_Models;
using MauiApp_Pesyong.Shared.Admin_Main.Admin_Services;

namespace MauiApp_Pesyong.Shared.Admin_Main.Admin_Pages;

public partial class AdminDashboard : ComponentBase
{
    [Inject]
    public AdminDataService DataService { get; set; } = default!;

    protected List<MealVm> meals = new();
    protected List<PackageVm> packages = new();
    protected List<DeliveryVm> deliveries = new();
    protected List<OrderVm> orders = new();

    protected override void OnInitialized()
    {
        meals = DataService.GetMeals();
        packages = DataService.GetPackages();
        deliveries = DataService.GetDeliveries();
        orders = DataService.GetOrders();
    }
}
