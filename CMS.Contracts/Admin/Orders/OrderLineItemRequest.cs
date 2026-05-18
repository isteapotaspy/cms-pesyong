using System;
using System.Collections.Generic;
using System.Text;

namespace CMS.Contracts.Admin.Orders;
public sealed record OrderLineItemRequest
{
    public int Id { get; init; }

    public string ItemType { get; init; } = string.Empty;

    public int? PackageId { get; init; }
    public int? PackageSizeId { get; init; }
    public int? MealId { get; init; }

    public string PackageTitleSnapshot { get; init; } = string.Empty;
    public string SizeLabelSnapshot { get; init; } = string.Empty;

    public decimal BaseUnitPrice { get; init; }
    public int Quantity { get; init; }

    public List<OrderItemMealSelectionRequest> MealSelections { get; init; } = new();
    public List<OrderItemAddonSelectionRequest> AddonSelections { get; init; } = new();
}