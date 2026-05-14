using System;
using System.Collections.Generic;
using System.Text;

namespace CMS.Contracts.Admin.Orders;

public class UpdateOrderStatusRequest
{
    public string Status { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
}
