using System;
using System.Collections.Generic;
using System.Text;

namespace CMS.Contracts.Customer.Orders;
public class PlaceOrderRequest
{
    public ContactInfoDto ContactInfo { get; set; } = new();
    public DeliveryAddressDto DeliveryAddress { get; set; } = new();
    public DeliveryScheduleDto DeliverySchedule { get; set; } = new();
    public string PaymentMethod { get; set; } = string.Empty;
    public string PromoCode { get; set; } = string.Empty;
    public decimal DeliveryFee { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal GrandTotal { get; set; }
    public List<OrderItemDto> Items { get; set; } = new();
}
