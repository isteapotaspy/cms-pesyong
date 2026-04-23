using Microsoft.AspNetCore.Components;
using MauiApp_Pesyong.Shared.Admin_Main.Admin_Models;
using MauiApp_Pesyong.Shared.Admin_Main.Admin_Services;

namespace MauiApp_Pesyong.Shared.Admin_Main.Admin_Pages;

public partial class Orders : ComponentBase
{
    [Inject]
    public AdminDataService DataService { get; set; } = default!;

    protected List<OrderVm> orders = new();
    protected OrderVm selectedOrder = new();
    protected string searchText = "";

    protected override void OnInitialized()
    {
        orders = DataService.GetOrders() ?? new();
        if (orders.Any()) SelectOrder(orders.First());
        else CreateNewOrder();
    }

    protected IEnumerable<OrderVm> FilteredOrders => orders.Where(x =>
        string.IsNullOrWhiteSpace(searchText) ||
        x.CustomerName.Contains(searchText, StringComparison.OrdinalIgnoreCase));

    protected void SelectOrder(OrderVm order) => selectedOrder = Clone(order);

    protected void CreateNewOrder() => selectedOrder = new OrderVm
    {
        OrderId = "ORD-" + Guid.NewGuid().ToString().Substring(0, 8).ToUpper(),
        DeliveryStatus = "Pending",
        OrderDate = DateTime.Today
    };

    protected void SaveOrder()
    {
        selectedOrder.CustomerName = $"{selectedOrder.FirstName} {selectedOrder.LastName}";
        var idx = orders.FindIndex(x => x.OrderId == selectedOrder.OrderId);
        if (idx != -1) orders[idx] = selectedOrder;
        else orders.Add(selectedOrder);
    }

    private OrderVm Clone(OrderVm o) => new OrderVm
    {
        OrderId = o.OrderId,
        FirstName = o.FirstName,
        LastName = o.LastName,
        CustomerName = o.CustomerName,
        Address = o.Address,
        Total = o.Total,
        DeliveryStatus = o.DeliveryStatus,
        DeliveryType = o.DeliveryType,
        PhoneNumber = o.PhoneNumber,
        Email = o.Email,
        TrackingNumber = o.TrackingNumber,
        EstimatedDeliveryDate = o.EstimatedDeliveryDate,
        CustomerNotes = o.CustomerNotes
    };
}
