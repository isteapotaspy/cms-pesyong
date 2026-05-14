using CMS.Domain.Common;
using CMS.Domain.Entities.Orders;
using CMS.Domain.Enums;

namespace CMS.Domain.Entities.Payment;

public class Payment : BaseEntity
{
    public int OrderId { get; set; }

    public string ExternalReference { get; set; } = string.Empty;
    public PaymentMethod PaymentMethod { get; set; }
    public PaymentStatus PaymentStatus { get; set; }

    public DateTime TimestampUtc { get; set; } = DateTime.UtcNow;
    public decimal Amount { get; set; }
    public string Description { get; set; } = string.Empty;

    public Order Order { get; set; } = default!;
}