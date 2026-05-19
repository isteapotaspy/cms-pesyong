using System;
using System.Collections.Generic;
using System.Text;

namespace CMS.Contracts.Admin.Package;

public sealed record PackageSelectionOptionDto
{
    public int Id { get; init; }
    public int MealId { get; init; }
    public string MealName { get; init; } = string.Empty;
    public decimal AdditionalPrice { get; init; }
    public bool IsDefault { get; init; }
}

