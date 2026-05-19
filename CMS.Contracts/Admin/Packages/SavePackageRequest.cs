using System;
using System.Collections.Generic;
using System.Text;

namespace CMS.Contracts.Admin.Packages;

public class SavePackageRequest
{
    public int CategoryId { get; set; }

    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string CardSummary { get; set; } = string.Empty;
    public string Badge { get; set; } = string.Empty;
    public string Notice { get; set; } = string.Empty;
    public string ServesLabel { get; set; } = string.Empty;
    public string InclusionText { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;

    public decimal Rating { get; set; }
    public int ReviewCount { get; set; }

    public bool IsAvailable { get; set; }
    public bool IsCustomizable { get; set; }

    public List<SavePackageSizeRequest> Sizes { get; set; } = new();
    public List<SavePackageAddonRequest> Addons { get; set; } = new();
}
