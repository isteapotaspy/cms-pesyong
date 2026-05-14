using CMS.Domain.Common;
using CMS.Domain.Entities.Menu;
using CMS.Domain.Entities.Packages;

namespace CMS.Domain.Entities.Orders;

public class OrderItemMealSelection : BaseEntity
{
    public int OrderItemId { get; set; }
    public int PackageSelectionRuleId { get; set; }
    public int MealId { get; set; }

    public string RuleTitleSnapshot { get; set; } = string.Empty;
    public string MealNameSnapshot { get; set; } = string.Empty;
    public decimal AdditionalPrice { get; set; }

    public OrderItem OrderItem { get; set; } = default!;
    public PackageSelectionRule PackageSelectionRule { get; set; } = default!;
    public Meal Meal { get; set; } = default!;
}