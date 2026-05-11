using System;
using System.Collections.Generic;
using System.Text;

namespace CMS.Contracts.Admin.Customers;

public class AdminCustomerOrderSummaryDto
{
    public int OrderId { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public DateTime OrderedAtUtc { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal GrandTotal { get; set; }
}
