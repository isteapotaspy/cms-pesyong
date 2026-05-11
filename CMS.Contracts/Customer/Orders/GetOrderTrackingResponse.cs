using System;
using System.Collections.Generic;
using System.Text;

namespace CMS.Contracts.Customer.Orders;

public class GetOrderTrackingResponse
{
    public int OrderId { get; set; }
    public string OrderNumber { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;
    public DateTime OrderedAtUtc { get; set; }
    public DateTime EstimatedDeliveryTimeUtc { get; set; }

    public DeliveryAddressDto DeliveryAddress { get; set; } = new();
    public RiderDto? Rider { get; set; }

    public decimal AmountPaid { get; set; }
    public List<TrackingOrderItemDto> Items { get; set; } = new();
    public List<TrackingStepDto> Steps { get; set; } = new();
}