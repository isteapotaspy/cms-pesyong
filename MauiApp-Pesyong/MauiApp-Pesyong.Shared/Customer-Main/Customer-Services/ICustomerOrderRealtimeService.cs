
using CMS.Contracts.Customer.Orders;

namespace MauiApp_Pesyong.Shared.Customer_Main.Customer_Services;

public interface ICustomerOrderRealtimeService : IAsyncDisposable
{
    event Action<OrderStatusUpdatedEvent>? OrderStatusUpdated;

    Task StartAsync(CancellationToken cancellationToken = default);
    Task StopAsync(CancellationToken cancellationToken = default);

    Task JoinOrderAsync(int orderId, CancellationToken cancellationToken = default);
    Task LeaveOrderAsync(int orderId, CancellationToken cancellationToken = default);

    Task JoinCustomerAsync(int customerProfileId, CancellationToken cancellationToken = default);
    Task LeaveCustomerAsync(int customerProfileId, CancellationToken cancellationToken = default);
}