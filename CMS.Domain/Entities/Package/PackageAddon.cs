using CMS.Domain.Common;

namespace CMS.Domain.Entities.Package;

public class PackageAddon : BaseEntity
{
    public int PackageId { get; set; }

    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public bool IsAvailable { get; set; } = true;

    public Package Package { get; set; } = default!;
}