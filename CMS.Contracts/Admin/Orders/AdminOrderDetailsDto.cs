using System;
using System.Collections.Generic;
using System.Text;

namespace CMS.Contracts.Admin.Orders;
public class AdminOrderDetailsDto
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

    public string CustomerNotes { get; set; } = string.Empty;
    public string SpecialInstructions { get; set; } = string.Empty;
    public string PromoCodeApplied { get; set; } = string.Empty;

    public AdminOrderAddressDto? DeliveryAddress { get; set; }

    public decimal SubTotal { get; set; }
    public decimal DeliveryFee { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal GrandTotal { get; set; }

    public List<AdminOrderItemDto> Items { get; set; } = new();
}
