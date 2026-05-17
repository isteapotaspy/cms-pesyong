using System;
using System.Collections.Generic;
using System.Text;

namespace CMS.Contracts.Customer.Menu;

public class PackageSelectionOptionDto
{
    public int Id { get; set; }
    public string? MealName { get; set; }
    public int MealId { get; set; }
    public decimal AdditionalPrice { get; set; }
    public bool IsDefault { get; set; }

    public MealOptionDto Meal { get; set; } = new();
}
