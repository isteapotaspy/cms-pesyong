using CMS.Domain.Common;
using CMS.Domain.Entities.Menu;

namespace CMS.Domain.Entities.Packages;

public class PackageSelectionOption : BaseEntity
{
    public int PackageSelectionRuleId { get; set; }
    public int MealId { get; set; }

    public decimal AdditionalPrice { get; set; } = 0m;
    public bool IsDefault { get; set; }

    public PackageSelectionRule PackageSelectionRule { get; set; } = default!;
    public Meal Meal { get; set; } = default!;
}