using System;
using System.Collections.Generic;
using System.Text;

namespace CMS.Contracts.Admin.Meals;

public sealed record CreateMealRequest
{
    public int CategoryId { get; init; }

    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string MealType { get; init; } = string.Empty;

    public decimal BasePrice { get; init; }
    public int StockQuantity { get; init; }
    public int MinOrderQuantity { get; init; } = 1;

    public string ImageUrl { get; init; } = string.Empty;
    public bool IsAvailable { get; init; } = true;
    public bool IsViandOption { get; init; }
}

