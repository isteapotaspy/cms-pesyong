using System;
using System.Collections.Generic;
using System.Text;

namespace CMS.Contracts.Admin.Orders;
public class AdminOrderListItemDto
{
    public int Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;

    public int CustomerProfileId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public string CustomerMobileNumber { get; set; } = string.Empty;

    public DateTime OrderedAtUtc { get; set; }
    public DateTime DeliveryDate { get; set; }
    public string DeliveryTimeSlot { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;
    public string PaymentMethod { get; set; } = string.Empty;
    public string PaymentStatus { get; set; } = string.Empty;

    public int ItemCount { get; set; }
    public decimal GrandTotal { get; set; }
}