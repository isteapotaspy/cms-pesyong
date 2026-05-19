using System;
using System.Collections.Generic;
using System.Text;
using CMS.Contracts.Customer.Menu;

namespace CMS.Contracts.Admin.Package;

public sealed record PackageSelectionRuleDto
{
    public int Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string SelectionType { get; init; } = string.Empty;
    public string AllowedMealType { get; init; } = string.Empty;
    public int MinSelections { get; init; }
    public int MaxSelections { get; init; }
    public bool IsRequired { get; init; }
    public int DisplayOrder { get; init; }

    public List<PackageSelectionOptionDto> Options { get; init; } = new();
}
