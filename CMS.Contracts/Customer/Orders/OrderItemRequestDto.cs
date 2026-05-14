using System;
using System.Collections.Generic;
using System.Text;

namespace CMS.Contracts.Customer.Orders;

public class OrderItemRequestDto
{
    public string ItemType { get; set; } = "Package"; // Package or Meal

    public int? PackageId { get; set; }
    public int? PackageSizeId { get; set; }

    public int? MealId { get; set; }

    public int Quantity { get; set; }

    public List<OrderItemMealSelectionRequestDto> MealSelections { get; set; } = new();
    public List<OrderItemAddonSelectionRequestDto> AddonSelections { get; set; } = new();
}
