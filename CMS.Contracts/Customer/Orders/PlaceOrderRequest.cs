using System;
using System.Collections.Generic;
using System.Text;

namespace CMS.Contracts.Customer.Orders;

public class PlaceOrderRequest
{
    public int? CustomerProfileId { get; set; }

    public ContactInfoDto ContactInfo { get; set; } = new();
    public DeliveryAddressDto DeliveryAddress { get; set; } = new();
    public DeliveryScheduleDto DeliverySchedule { get; set; } = new();

    public string PaymentMethod { get; set; } = string.Empty;
    public string PromoCode { get; set; } = string.Empty;
    public string CustomerNotes { get; set; } = string.Empty;
    public string SpecialInstructions { get; set; } = string.Empty;

    public List<OrderItemRequestDto> Items { get; set; } = new();
}
