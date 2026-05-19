using System;
using System.Collections.Generic;
using System.Text;
using CMS.Contracts.Customer.Menu;

namespace CMS.Contracts.Admin.Package;

public sealed record PackageSizeDto
{
    public int Id { get; init; }
    public string Label { get; init; } = string.Empty;
    public string Subtitle { get; init; } = string.Empty;
    public int PaxCount { get; init; }
    public decimal Price { get; init; }

    public List<PackageSelectionRuleDto> SelectionRules { get; init; } = new();
}
