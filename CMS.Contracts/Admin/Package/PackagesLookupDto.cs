using System;
using System.Collections.Generic;
using System.Text;

namespace CMS.Contracts.Admin.Package;

public sealed record PackageLookupDto
{
    public List<LookupItemDto> MenuCategories { get; init; } = new();
    public List<LookupItemDto> Meals { get; init; } = new();
    public List<string> PackageSelectionTypes { get; init; } = new();
    public List<string> MealTypes { get; init; } = new();
}

