
namespace CMS.Contracts.Customer.Orders;

public class CustomerOrderListItemDto
{
    public int Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public DateTime OrderedAtUtc { get; set; }
    public DateTime DeliveryDate { get; set; }
    public string DeliveryTimeSlot { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string PaymentStatus { get; set; } = string.Empty;
    public decimal GrandTotal { get; set; }
    public int ItemCount { get; set; }
}