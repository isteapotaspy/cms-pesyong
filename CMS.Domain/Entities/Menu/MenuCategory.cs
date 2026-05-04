using CMS.Domain.Common;

namespace CMS.Domain.Entities.Menu;

public class MenuCategory : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;

    public ICollection<Package> Packages { get; set; } = new List<Package>();
    public ICollection<Meal> Meals { get; set; } = new List<Meal>();
}