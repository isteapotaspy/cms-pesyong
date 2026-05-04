using CMS.Domain.Common;
using CMS.Domain.Entities.User;
using CMS.Domain.Enums;

namespace CMS.Domain.Entities.Payment;

public class DeliveryUpdate : BaseEntity
{
    public int DeliveryId { get; set; }
    public int? UpdatedByUserId { get; set; }

    public DeliveryStatus Status { get; set; }
    public DateTime UpdateDateUtc { get; set; } = DateTime.UtcNow;
    public string UpdateDescription { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;

    public Delivery Delivery { get; set; } = default!;
    public AppUser? UpdatedByUser { get; set; }
}