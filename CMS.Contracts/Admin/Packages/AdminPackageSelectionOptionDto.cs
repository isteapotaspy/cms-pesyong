using System;
using System.Collections.Generic;
using System.Text;

namespace CMS.Contracts.Admin.Packages;
public class AdminPackageSelectionOptionDto
{
    public int Id { get; set; }
    public int MealId { get; set; }
    public string MealName { get; set; } = string.Empty;
    public string MealType { get; set; } = string.Empty;
    public decimal AdditionalPrice { get; set; }
    public bool IsDefault { get; set; }
}