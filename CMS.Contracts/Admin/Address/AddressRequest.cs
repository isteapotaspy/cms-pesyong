using System;
using System.Collections.Generic;
using System.Text;

namespace CMS.Contracts.Admin.Address;

public sealed class AddressRequest
{
    public int CustomerProfileId { get; set; }

    public string StreetAddress { get; set; } = string.Empty;
    public string Barangay { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Landmark { get; set; } = string.Empty;

    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }

    public bool IsDefault { get; set; }
}