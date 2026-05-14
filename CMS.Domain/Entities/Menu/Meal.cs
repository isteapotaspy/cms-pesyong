using CMS.Domain.Common;
using CMS.Domain.Entities.Packages;
using CMS.Domain.Enums;

namespace CMS.Domain.Entities.Menu;

public class Meal : BaseEntity
{
    public int MenuCategoryId { get; set; }

    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public MealType MealType { get; set; }

    public decimal BasePrice { get; set; }
    public int StockQuantity { get; set; }
    public int MinOrderQuantity { get; set; } = 1;

    public string ImageUrl { get; set; } = string.Empty;
    public bool IsAvailable { get; set; } = true;
    public bool IsViandOption { get; set; }

    public MenuCategory MenuCategory { get; set; } = default!;
    public ICollection<PackageSelectionOption> PackageSelectionOptions { get; set; } = new List<PackageSelectionOption>();
}