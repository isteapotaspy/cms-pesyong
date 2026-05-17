using System;
using System.Collections.Generic;
using System.Text;

namespace CMS.Contracts.Admin.Packages;

public class SavePackageSelectionRuleRequest
{
    public int? Id { get; set; }

    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public string SelectionType { get; set; } = string.Empty;
    public string AllowedMealType { get; set; } = string.Empty;

    public int MinSelections { get; set; }
    public int MaxSelections { get; set; }
    public bool IsRequired { get; set; }
    public int DisplayOrder { get; set; }

    public List<SavePackageSelectionOptionRequest> Options { get; set; } = new();
}