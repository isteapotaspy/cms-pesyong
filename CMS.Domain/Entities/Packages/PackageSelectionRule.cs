using CMS.Domain.Common;
using CMS.Domain.Enums;

namespace CMS.Domain.Entities.Packages;

public class PackageSelectionRule : BaseEntity
{
    public int PackageSizeId { get; set; }

    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public PackageSelectionType SelectionType { get; set; }
    public MealType AllowedMealType { get; set; }

    public int MinSelections { get; set; } 
    public int MaxSelections { get; set; }
    public bool IsRequired { get; set; } = true;
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;

    public PackageSize PackageSize { get; set; } = default!;
    public ICollection<PackageSelectionOption> Options { get; set; } = new List<PackageSelectionOption>();
}