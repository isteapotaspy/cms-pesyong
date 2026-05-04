using System;
using System.Collections.Generic;
using System.Text;

namespace CMS.Contracts.Customer.Orders;
public class OrderItemDto
{
    public string PackageId { get; set; } = string.Empty;
    public string PackageTitle { get; set; } = string.Empty;
    public string SizeLabel { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
}