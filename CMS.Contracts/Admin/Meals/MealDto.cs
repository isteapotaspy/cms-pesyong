using System;
using System.Collections.Generic;
using System.Text;

namespace CMS.Contracts.Admin.Meals;

public sealed record MealDto
{
    public int Id { get; init; }
    public int CategoryId { get; init; }
    public string CategoryName { get; init; } = string.Empty;

    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string MealType { get; init; } = string.Empty;

    public decimal BasePrice { get; init; }
    public int StockQuantity { get; init; }
    public int MinOrderQuantity { get; init; }

    public string ImageUrl { get; init; } = string.Empty;
    public bool IsAvailable { get; init; }
    public bool IsViandOption { get; init; }
}
