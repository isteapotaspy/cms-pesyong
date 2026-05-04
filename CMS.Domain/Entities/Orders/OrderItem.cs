using CMS.Domain.Common;
using CMS.Domain.Entities.Package;

namespace CMS.Domain.Entities.Orders;

public class OrderItem : BaseEntity
{
    public int OrderId { get; set; }
    public int PackageId { get; set; }
    public int? PackageSizeId { get; set; }

    public string PackageTitleSnapshot { get; set; } = string.Empty;
    public string SizeLabelSnapshot { get; set; } = string.Empty;

    public decimal BaseUnitPrice { get; set; }
    public decimal CustomizationsTotal { get; set; }
    public int Quantity { get; set; }

    public Order Order { get; set; } = default!;
    public Package Package { get; set; } = default!;
    public PackageSize? PackageSize { get; set; }

    public ICollection<OrderItemMealSelection> MealSelections { get; set; } = new List<OrderItemMealSelection>();
    public ICollection<OrderItemAddonSelection> AddonSelections { get; set; } = new List<OrderItemAddonSelection>();

    public decimal UnitPrice => BaseUnitPrice + CustomizationsTotal;
    public decimal LineTotal => UnitPrice * Quantity;
}