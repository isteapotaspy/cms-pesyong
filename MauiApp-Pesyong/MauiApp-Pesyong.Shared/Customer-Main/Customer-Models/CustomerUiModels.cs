namespace MauiApp_Pesyong.Shared.Customer_Main.Customer_Models;

public class PackageUiModel
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Title { get; set; } = string.Empty;
    public string MenuCategory { get; set; } = "Catering";
    public string CategoryLabel { get; set; } = "Catering Packages";
    public string Description { get; set; } = string.Empty;
    public string CardSummary { get; set; } = string.Empty;
    public string Badge { get; set; } = string.Empty;
    public string Notice { get; set; } = string.Empty;
    public string ServesLabel { get; set; } = string.Empty;
    public string InclusionText { get; set; } = string.Empty;
    public string ImageClass { get; set; } = "food-image-one";
    public decimal Rating { get; set; } = 4.8m;
    public int ReviewCount { get; set; } = 124;
    public List<PackageSizeUiModel> Sizes { get; set; } = new();
    public List<AddonUiModel> Addons { get; set; } = new();
}

public class PackageSizeUiModel
{
    public string Label { get; set; } = string.Empty;
    public string Subtitle { get; set; } = string.Empty;
    public decimal Price { get; set; }
}

public class AddonUiModel
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
}

public class CartLineUiModel
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; } = 1;
    public string ImageClass { get; set; } = "food-image-one";
    public decimal LineTotal => UnitPrice * Quantity;
}