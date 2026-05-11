using System;
using System.Collections.Generic;
using System.Text;

namespace CMS.Contracts.Customer.Menu;

public class MealOptionDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string MealType { get; set; } = string.Empty;
    public decimal BasePrice { get; set; }
    public decimal AdditionalPrice { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public bool IsDefault { get; set; }
}
