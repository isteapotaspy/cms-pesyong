using System;
using System.Collections.Generic;
using System.Text;

namespace CMS.Contracts.Admin.Customers;

public sealed class CustomerAddressLookupDto
{
    public int CustomerProfileId { get; set; }
    public int AddressId { get; set; }

    public string CustomerDetails { get; set; } = string.Empty;
    public string AddressDetails { get; set; } = string.Empty;
}