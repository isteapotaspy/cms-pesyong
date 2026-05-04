using CMS.Domain.Common;
using CMS.Domain.Entities.Package;

namespace CMS.Domain.Entities.Orders;

public class OrderItemAddonSelection : BaseEntity
{
    public int OrderItemId { get; set; }
    public int PackageAddonId { get; set; }

    public string AddonNameSnapshot { get; set; } = string.Empty;
    public decimal AdditionalPrice { get; set; }

    public OrderItem OrderItem { get; set; } = default!;
    public PackageAddon PackageAddon { get; set; } = default!;
}