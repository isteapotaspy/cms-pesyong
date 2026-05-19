using System;
using System.Collections.Generic;
using System.Text;

namespace CMS.Contracts.Admin.Packages;

public class AdminPackageSizeDto
{
    public int Id { get; set; }
    public string Label { get; set; } = string.Empty;
    public string Subtitle { get; set; } = string.Empty;
    public int PaxCount { get; set; }
    public decimal Price { get; set; }

    public List<AdminPackageSelectionRuleDto> SelectionRules { get; set; } = new();
}
