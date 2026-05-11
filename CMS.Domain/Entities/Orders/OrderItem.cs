using CMS.Domain.Common;
using CMS.Domain.Entities.Menu;
using CMS.Domain.Entities.Packages;
using CMS.Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CMS.Domain.Entities.Orders;

public class OrderItem
{
    [Key]
    public int Id { get; set; }

    [ForeignKey(nameof(Order))]
    public int OrderId { get; set; }
    public Order? Order { get; set; }

    public OrderItemType ItemType { get; set; } = OrderItemType.Package;

    // Package-based item
    public int? PackageId { get; set; }
    public int? PackageSizeId { get; set; }
    public Package? Package { get; set; }
    public PackageSize? PackageSize { get; set; }

    // Meal-based item
    public int? MealId { get; set; }
    public Meal? Meal { get; set; }

    // snapshots for history
    [Required]
    [StringLength(200)]
    public string PackageTitleSnapshot { get; set; } = string.Empty;

    [StringLength(100)]
    public string SizeLabelSnapshot { get; set; } = string.Empty;

    [Column(TypeName = "decimal(18,2)")]
    public decimal BaseUnitPrice { get; set; }

    [Required]
    [Range(1, 1000)]
    public int Quantity { get; set; }

    public List<OrderItemMealSelection> MealSelections { get; set; } = new();
    public List<OrderItemAddonSelection> AddonSelections { get; set; } = new();

    [NotMapped]
    public decimal UnitPrice =>
        BaseUnitPrice
        + MealSelections.Sum(x => x.AdditionalPrice)
        + AddonSelections.Sum(x => x.AdditionalPrice);

    [NotMapped]
    public decimal LineTotal => UnitPrice * Quantity;
}