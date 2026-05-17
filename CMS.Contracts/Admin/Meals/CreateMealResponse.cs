using System;
using System.Collections.Generic;
using System.Text;

namespace CMS.Contracts.Admin.Meals;

public class CreateMealResponse
{
    public int Id { get; set; }
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string MealType { get; set; } = string.Empty;

    public decimal BasePrice { get; set; }
    public int StockQuantity { get; set; }
    public int MinOrderQuantity { get; set; }

    public string ImageUrl { get; set; } = string.Empty;
    public bool IsAvailable { get; set; }
    public bool IsViandOption { get; set; }
}
