using System;
using System.Collections.Generic;
using System.Text;

namespace CMS.Contracts.Customer.Orders;

public class PlaceOrderResponse
{
    public int OrderId { get; set; }
    public string OrderNumber { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;
    public DateTime OrderedAtUtc { get; set; }
    public DateTime EstimatedDeliveryTimeUtc { get; set; }

    public decimal SubTotal { get; set; }
    public decimal DeliveryFee { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal GrandTotal { get; set; }
}
