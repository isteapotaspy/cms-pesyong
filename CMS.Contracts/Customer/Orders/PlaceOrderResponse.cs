using System;
using System.Collections.Generic;
using System.Text;

namespace CMS.Contracts.Customer.Orders;
public class PlaceOrderResponse
{
    public string OrderId { get; set; } = string.Empty;
    public string OrderNumber { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime OrderedAt { get; set; }
    public DateTime EstimatedDeliveryTime { get; set; }
    public decimal AmountPaid { get; set; }
}
