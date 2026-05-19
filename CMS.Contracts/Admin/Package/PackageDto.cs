using System;
using System.Collections.Generic;
using System.Text;
using CMS.Contracts.Customer.Menu;

namespace CMS.Contracts.Admin.Package;

public sealed record PackageDto
{
    public int Id { get; init; }
    public DateTime? CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }

    public int MenuCategoryId { get; init; }
    public string MenuCategoryName { get; init; } = string.Empty;

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
    public bool IsAvailable { get; init; }
    public bool IsCustomizable { get; init; }

    public List<PackageSizeDto> Sizes { get; init; } = new();
    public List<PackageAddonDto> Addons { get; init; } = new();
}