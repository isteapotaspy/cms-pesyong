using System;
using System.Collections.Generic;
using System.Text;

namespace CMS.Contracts.Admin.Customers;

public class AdminCustomerListItemDto
{
    public int CustomerProfileId { get; set; }
    public int AppUserId { get; set; }

    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string MobileNumber { get; set; } = string.Empty;

    public int TotalOrders { get; set; }
    public decimal TotalSpent { get; set; }
    public DateTime? LastOrderDateUtc { get; set; }
}
