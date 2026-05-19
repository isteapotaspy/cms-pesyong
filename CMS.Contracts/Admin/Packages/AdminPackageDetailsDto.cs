using CMS.Contracts.Admin.Packages;

public class AdminPackageDetailsDto
{
    public int Id { get; set; }
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;

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

    public List<AdminPackageSizeDto> Sizes { get; set; } = new();
    public List<AdminPackageAddonDto> Addons { get; set; } = new();
}