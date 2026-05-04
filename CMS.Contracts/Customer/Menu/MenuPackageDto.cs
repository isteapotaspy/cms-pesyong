using System;
using System.Collections.Generic;
using System.Text;

namespace CMS.Contracts.Customer.Menu;
public class MenuPackageDto
{
    public string Id { get; set; } = string.Empty;
    public string CategoryId { get; set; } = string.Empty;
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
    public List<PackageSizeDto> Sizes { get; set; } = new();
    public List<AddonDto> Addons { get; set; } = new();
}
