using System;
using System.Collections.Generic;
using System.Text;

namespace CMS.Contracts.Admin.Orders;

public sealed record OrderLineItemDto
{
    public int Id { get; init; }

    public int OrderId { get; init; }

    public string ItemType { get; init; } = string.Empty;

    public int? PackageId { get; init; }
    public int? PackageSizeId { get; init; }
    public int? MealId { get; init; }

    public string PackageTitleSnapshot { get; init; } = string.Empty;
    public string SizeLabelSnapshot { get; init; } = string.Empty;

    public decimal BaseUnitPrice { get; init; }
    public int Quantity { get; init; }

    public decimal UnitPrice { get; init; }
    public decimal LineTotal { get; init; }

    public List<OrderItemMealSelectionRequestDto> MealSelections { get; init; } = new();
    public List<OrderItemAddonSelectionDto> AddonSelections { get; init; } = new();
}