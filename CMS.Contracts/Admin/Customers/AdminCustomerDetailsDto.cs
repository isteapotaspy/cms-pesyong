using System;
using System.Collections.Generic;
using System.Text;

namespace CMS.Contracts.Admin.Customers;

public class AdminCustomerDetailsDto
{
    public int CustomerProfileId { get; set; }
    public int AppUserId { get; set; }

    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;
    public string MobileNumber { get; set; } = string.Empty;
    public bool IsActive { get; set; }

    public DateTime CreatedAtUtc { get; set; }

    public int TotalOrders { get; set; }
    public decimal TotalSpent { get; set; }

    public List<AdminCustomerAddressDto> Addresses { get; set; } = new();
    public List<AdminCustomerOrderSummaryDto> RecentOrders { get; set; } = new();
}
