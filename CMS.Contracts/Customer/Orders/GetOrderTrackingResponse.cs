using System;
using System.Collections.Generic;
using System.Text;

namespace CMS.Contracts.Customer.Orders;
public class GetOrderTrackingResponse
{
    public string OrderId { get; set; } = string.Empty;
    public string OrderNumber { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime OrderedAt { get; set; }
    public DateTime EstimatedDeliveryTime { get; set; }
    public RiderDto Rider { get; set; } = new();
    public DeliveryAddressDto DeliveryAddress { get; set; } = new();
    public List<TrackingStepDto> Steps { get; set; } = new();
    public List<TrackingOrderItemDto> Items { get; set; } = new();
    public decimal AmountPaid { get; set; }
}