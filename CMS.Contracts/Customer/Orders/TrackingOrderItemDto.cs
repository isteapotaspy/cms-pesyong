using System;
using System.Collections.Generic;
using System.Text;

namespace CMS.Contracts.Customer.Orders;
public class TrackingOrderItemDto
{
    public string Name { get; set; } = string.Empty;
    public string Subtitle { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
}
