using System;
using System.Collections.Generic;
using System.Text;

namespace CMS.Contracts.Admin.Package;

public sealed record PackageSelectionOptionRequest
{
    public int MealId { get; init; }
    public decimal AdditionalPrice { get; init; }
    public bool IsDefault { get; init; }
}