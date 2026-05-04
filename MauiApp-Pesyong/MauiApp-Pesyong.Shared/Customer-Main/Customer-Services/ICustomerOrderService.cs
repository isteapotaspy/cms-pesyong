using CMS.Contracts.Customer.Orders;
using MauiApp_Pesyong.Shared.Customer_Main.Customer_Vms;

namespace MauiApp_Pesyong.Shared.Customer_Main.Customer_Services;

public interface ICustomerOrderService
{
    Task<PlaceOrderResponse> PlaceOrderAsync(CheckoutVm vm, CancellationToken cancellationToken = default);
    Task<TrackingVm?> GetTrackingAsync(string orderId, CancellationToken cancellationToken = default);
}