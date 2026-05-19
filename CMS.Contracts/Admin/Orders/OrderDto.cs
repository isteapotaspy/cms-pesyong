using System;
using System.Collections.Generic;
using System.Text;

namespace CMS.Contracts.Admin.Orders;

public sealed record OrderDto
{
    public int Id { get; init; }
    public DateTime DateCreated { get; init; }
    public DateTime? DateUpdated { get; init; }

    public string OrderNumber { get; init; } = string.Empty;

    public int CustomerProfileId { get; init; }
    public int? AddressId { get; init; }

    public DateTime OrderedAtUtc { get; init; }
    public DateTime DeliveryDate { get; init; }
    public string DeliveryTimeSlot { get; init; } = string.Empty;

    public string Status { get; init; } = string.Empty;
    public string PaymentMethod { get; init; } = string.Empty;
    public string PaymentStatus { get; init; } = string.Empty;

    public string ContactNameSnapshot { get; init; } = string.Empty;
    public string ContactEmailSnapshot { get; init; } = string.Empty;
    public string ContactMobileSnapshot { get; init; } = string.Empty;

    public string CustomerNotes { get; init; } = string.Empty;
    public string SpecialInstructions { get; init; } = string.Empty;

    public string PromoCodeApplied { get; init; } = string.Empty;

    public decimal SubTotal { get; init; }
    public decimal DeliveryFee { get; init; }
    public decimal TaxAmount { get; init; }
    public decimal DiscountAmount { get; init; }
    public decimal GrandTotal { get; init; }

    public List<OrderLineItemDto> Items { get; init; } = new();
}

