
namespace CMS.Contracts.Customer.Orders;

public class OrderStatusUpdatedEvent
{
    public int OrderId { get; set; }
    public int CustomerProfileId { get; set; }
    public string OrderNumber { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;
    public string PaymentStatus { get; set; } = string.Empty;

    public DateTime UpdatedAtUtc { get; set; }
}