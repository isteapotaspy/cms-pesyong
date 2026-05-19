using CMS.Domain.Common;
using CMS.Domain.Entities.Orders;
using CMS.Domain.Entities.User;
using CMS.Domain.Enums;

namespace CMS.Domain.Entities.Orders;

public class Order : BaseEntity
{
    public string OrderNumber { get; set; } = string.Empty;

    public int CustomerProfileId { get; set; }
    public int? AddressId { get; set; }

    public DateTime OrderedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime DeliveryDate { get; set; }
    public string DeliveryTimeSlot { get; set; } = string.Empty;

    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public PaymentMethod PaymentMethod { get; set; }
    public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;

    public string ContactNameSnapshot { get; set; } = string.Empty;
    public string ContactEmailSnapshot { get; set; } = string.Empty;
    public string ContactMobileSnapshot { get; set; } = string.Empty;

    public string CustomerNotes { get; set; } = string.Empty;
    public string SpecialInstructions { get; set; } = string.Empty;

    public string PromoCodeApplied { get; set; } = string.Empty;

    public decimal SubTotal { get; set; }
    public decimal DeliveryFee { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal GrandTotal { get; set; }

    public CustomerProfile CustomerProfile { get; set; } = default!;
    public Address? Address { get; set; }

    public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();

}