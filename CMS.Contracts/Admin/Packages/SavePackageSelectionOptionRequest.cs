using System;
using System.Collections.Generic;
using System.Text;

namespace CMS.Contracts.Admin.Packages;

public class SavePackageSelectionOptionRequest
{
    public int? Id { get; set; }
    public int MealId { get; set; }
    public decimal AdditionalPrice { get; set; }
    public bool IsDefault { get; set; }
}
