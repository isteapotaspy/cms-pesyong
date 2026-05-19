using System;
using System.Collections.Generic;
using System.Text;

namespace CMS.Contracts.Admin.Package;

public sealed record UpdatePackageRequest
{
    public int Id { get; init; }
    public int MenuCategoryId { get; init; }

    public string Title { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string CardSummary { get; init; } = string.Empty;
    public string Badge { get; init; } = string.Empty;
    public string Notice { get; init; } = string.Empty;
    public string ServesLabel { get; init; } = string.Empty;
    public string InclusionText { get; init; } = string.Empty;
    public string ImageUrl { get; init; } = string.Empty;

    public decimal Rating { get; init; }
    public int ReviewCount { get; init; }
    public bool IsAvailable { get; init; } = true;
    public bool IsCustomizable { get; init; }

    public List<PackageSizeRequest> Sizes { get; init; } = new();
    public List<PackageAddonRequest> Addons { get; init; } = new();
}

