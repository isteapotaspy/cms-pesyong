using System;
using System.Collections.Generic;
using System.Text;

namespace CMS.Contracts.Admin.Package;

public sealed record PackageSelectionRuleRequest
{
    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string SelectionType { get; init; } = string.Empty;
    public string AllowedMealType { get; init; } = string.Empty;
    public int MinSelections { get; init; }
    public int MaxSelections { get; init; }
    public bool IsRequired { get; init; }
    public int DisplayOrder { get; init; }

    public List<PackageSelectionOptionRequest> Options { get; init; } = new();
}
