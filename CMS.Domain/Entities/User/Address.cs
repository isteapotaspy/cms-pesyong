using CMS.Domain.Common;

namespace CMS.Domain.Entities.User;

public class Address : BaseEntity
{
    public int CustomerProfileId { get; set; }

    public string StreetAddress { get; set; } = string.Empty;
    public string Barangay { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Landmark { get; set; } = string.Empty;
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public bool IsDefault { get; set; }

    public CustomerProfile CustomerProfile { get; set; } = default!;
}