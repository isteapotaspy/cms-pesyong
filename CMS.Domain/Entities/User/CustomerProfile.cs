using CMS.Domain.Common;
using CMS.Domain.Entities.Orders;
using System.Net;

namespace CMS.Domain.Entities.User;

public class CustomerProfile : BaseEntity
{
    public int AppUserId { get; set; }
    public string MobileNumber { get; set; } = string.Empty;

    public AppUser AppUser { get; set; } = default!;
    public ICollection<Address> Addresses { get; set; } = new List<Address>();
    public ICollection<Order> Orders { get; set; } = new List<Order>();
}