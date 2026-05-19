using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace CMS.Server.Hubs;

[Authorize]
public class OrderHub : Hub
{
    public async Task JoinOrderGroup(int orderId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, GetOrderGroup(orderId));
    }

    public async Task LeaveOrderGroup(int orderId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, GetOrderGroup(orderId));
    }

    public async Task JoinCustomerGroup(int customerProfileId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, GetCustomerGroup(customerProfileId));
    }

    public async Task LeaveCustomerGroup(int customerProfileId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, GetCustomerGroup(customerProfileId));
    }

    public static string GetOrderGroup(int orderId) => $"order-{orderId}";
    public static string GetCustomerGroup(int customerProfileId) => $"customer-{customerProfileId}";
}