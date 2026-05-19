using CMS.Domain.Common;
using CMS.Domain.Entities.Menu;

namespace CMS.Domain.Entities.Packages;

public class Package : BaseEntity
{

    public int MenuCategoryId { get; set; }

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
    public bool IsAvailable { get; set; } = true;
    public bool IsCustomizable { get; set; }

    public MenuCategory MenuCategory { get; set; } = default!;
    public ICollection<PackageSize> Sizes { get; set; } = new List<PackageSize>();
    public ICollection<PackageAddon> Addons { get; set; } = new List<PackageAddon>();
}