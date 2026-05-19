using CMS.Contracts.Customer.Orders;
using CMS.Contracts.Customer.Promos;
using MauiApp_Pesyong.Shared.Customer_Main.Customer_Vms;

namespace MauiApp_Pesyong.Shared.Customer_Main.Customer_Services;

public interface ICustomerOrderService
{
    Task<IReadOnlyList<CustomerOrderListItemDto>> GetMyOrdersAsync(CancellationToken cancellationToken = default);
    Task<PlaceOrderResponse> PlaceOrderAsync(CheckoutVm vm, CancellationToken cancellationToken = default);
    Task<TrackingVm?> GetTrackingAsync(int orderId, CancellationToken cancellationToken = default);
    Task<DownloadedFileVm?> DownloadInvoiceAsync(int orderId, CancellationToken cancellationToken = default);
    Task<PromoValidationResponse?> ValidatePromoAsync(string code, decimal subTotal, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ActivePromoDto>> GetActivePromosAsync(CancellationToken cancellationToken = default);
}