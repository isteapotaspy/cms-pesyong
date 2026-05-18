using System;
using System.Collections.Generic;
using System.Text;
using CMS.Domain.Enums;

namespace CMS.Contracts.Admin.Deliveries;

public sealed record class CreateDeliveryRequest
{
    public int OrderId { get; init; }
    public int? RiderUserId { get; init; }

    public DeliveryStatus Status { get; init; }

    public string DeliveryAddressSnapshot { get; init; } = string.Empty;
    public decimal ShippingCost { get; init; }

    public string TrackingNumber { get; init; } = string.Empty;
    public string CurrentLocation { get; init; } = string.Empty;

    public DateTime? EstimatedDeliveryDateUtc { get; init; }
    public DateTime? ActualDeliveryDateUtc { get; init; }
}
