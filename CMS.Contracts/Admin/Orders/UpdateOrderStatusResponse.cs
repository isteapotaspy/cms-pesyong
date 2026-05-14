using System;
using System.Collections.Generic;
using System.Text;

namespace CMS.Contracts.Admin.Orders;

public class UpdateOrderStatusResponse
{
    public int OrderId { get; set; }
    public string OrderNumber { get; set; } = string.Empty;

    public string PreviousStatus { get; set; } = string.Empty;
    public string CurrentStatus { get; set; } = string.Empty;

    public DateTime UpdatedAtUtc { get; set; }
    public string Notes { get; set; } = string.Empty;
}
