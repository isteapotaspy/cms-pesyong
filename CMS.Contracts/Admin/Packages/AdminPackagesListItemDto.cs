using System;
using System.Collections.Generic;
using System.Text;

namespace CMS.Contracts.Admin.Packages;

public class AdminPackageListItemDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string CategoryName { get; set; } = string.Empty;
    public string Badge { get; set; } = string.Empty;
    public string Notice { get; set; } = string.Empty;
    public string ServesLabel { get; set; } = string.Empty;

    public decimal Rating { get; set; }
    public int ReviewCount { get; set; }

    public bool IsAvailable { get; set; }
    public bool IsCustomizable { get; set; }

    public int SizeCount { get; set; }
    public int AddonCount { get; set; }
    public int SelectionRuleCount { get; set; }
}