using CMS.Domain.Common;
using CMS.Domain.Entities.Orders;
using CMS.Domain.Entities.User;
using CMS.Domain.Enums;

namespace CMS.Domain.Entities.Payment;

public class Delivery : BaseEntity
{
    public int OrderId { get; set; }
    public int? RiderUserId { get; set; }

    public DeliveryStatus Status { get; set; } = DeliveryStatus.Pending;
    public string DeliveryAddressSnapshot { get; set; } = string.Empty;
    public decimal ShippingCost { get; set; }

    public string TrackingNumber { get; set; } = string.Empty;
    public string CurrentLocation { get; set; } = string.Empty;
    public DateTime? EstimatedDeliveryDateUtc { get; set; }
    public DateTime? ActualDeliveryDateUtc { get; set; }

    public Order Order { get; set; } = default!;
    public AppUser? RiderUser { get; set; }
    public ICollection<DeliveryUpdate> Updates { get; set; } = new List<DeliveryUpdate>();
}