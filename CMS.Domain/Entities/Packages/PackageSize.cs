using CMS.Domain.Common;

namespace CMS.Domain.Entities.Packages;

public class PackageSize : BaseEntity
{
    public int PackageId { get; set; }

    public string Label { get; set; } = string.Empty;
    public string Subtitle { get; set; } = string.Empty;
    public int PaxCount { get; set; }
    public decimal Price { get; set; }
    public bool IsAvailable { get; set; } = true;

    public Package Package { get; set; } = default!;
    public ICollection<PackageSelectionRule> SelectionRules { get; set; } = new List<PackageSelectionRule>();
}