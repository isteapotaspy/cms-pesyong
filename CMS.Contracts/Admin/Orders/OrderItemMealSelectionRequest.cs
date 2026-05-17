using System;
using System.Collections.Generic;
using System.Text;

namespace CMS.Contracts.Admin.Orders;

public sealed record OrderItemMealSelectionRequest
{
    public int Id { get; init; }

    public int PackageSelectionRuleId { get; init; }
    public int MealId { get; init; }

    public string RuleTitleSnapshot { get; init; } = string.Empty;
    public string MealNameSnapshot { get; init; } = string.Empty;
    public decimal AdditionalPrice { get; init; }
}