using System;
using System.Collections.Generic;
using System.Text;

namespace CMS.Contracts.Admin.Customers;

public class AdminCustomerAddressDto
{
    public int Id { get; set; }
    public string StreetAddress { get; set; } = string.Empty;
    public string Barangay { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Landmark { get; set; } = string.Empty;
    public bool IsDefault { get; set; }

    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
}
