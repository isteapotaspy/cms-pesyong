namespace MauiApp_Pesyong.Shared.Customer_Main.Customer_Models;

public class PackageUiModel
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public int PackageId { get; set; }

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

    public bool IsAvailable { get; set; }
    public bool IsCustomizable { get; set; }

    public List<PackageSizeUiModel> Sizes { get; set; } = new();
    public List<AddonUiModel> Addons { get; set; } = new();
    public List<PackageSelectionRuleUiModel> SelectionRules { get; set; } = new();
}

public class PackageSizeUiModel
{
    public int Id { get; set; }
    public string Label { get; set; } = string.Empty;
    public string Subtitle { get; set; } = string.Empty;
    public int PaxCount { get; set; }
    public decimal Price { get; set; }
    public List<PackageSelectionRuleUiModel> SelectionRules { get; set; } = new();
}

public class AddonUiModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public bool IsAvailable { get; set; }
}

public class PackageSelectionRuleUiModel
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string SelectionType { get; set; } = string.Empty;
    public string AllowedMealType { get; set; } = string.Empty;
    public int MinSelections { get; set; }
    public int MaxSelections { get; set; }
    public bool IsRequired { get; set; }
    public int DisplayOrder { get; set; }

    public List<PackageSelectionOptionUiModel> Options { get; set; } = new();
}

public class PackageSelectionOptionUiModel
{
    public int Id { get; set; }
    public int MealId { get; set; }
    public decimal AdditionalPrice { get; set; }
    public bool IsDefault { get; set; }

    public MealOptionUiModel Meal { get; set; } = new();
}

public class MealOptionUiModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string MealType { get; set; } = string.Empty;
    public decimal BasePrice { get; set; }
    public decimal AdditionalPrice { get; set; }
    public string ImageClass { get; set; } = "food-image-one";
    public bool IsDefault { get; set; }
}

public class CartLineUiModel
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public int PackageId { get; set; }
    public int? PackageSizeId { get; set; }

    public bool IsMealItem { get; set; }
    public int? MealId { get; set; }

    public string Name { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public string SizeLabel { get; set; } = string.Empty;

    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; } = 1;
    public string ImageClass { get; set; } = "food-image-one";

    public List<CartMealSelectionUiModel> MealSelections { get; set; } = new();
    public List<CartAddonSelectionUiModel> AddonSelections { get; set; } = new();

    public decimal LineTotal => UnitPrice * Quantity;
}

public class CartMealSelectionUiModel
{
    public int PackageSelectionRuleId { get; set; }
    public int MealId { get; set; }
    public string MealName { get; set; } = string.Empty;
    public decimal AdditionalPrice { get; set; }
}

public class CartAddonSelectionUiModel
{
    public int PackageAddonId { get; set; }
    public string AddonName { get; set; } = string.Empty;
    public decimal AdditionalPrice { get; set; }
}

public class ShortOrderMealUiModel
{
    public int Id { get; set; }
    public int CategoryId { get; set; }

    public string CategoryName { get; set; } = string.Empty;
    public string CategorySlug { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string MealType { get; set; } = string.Empty;

    public decimal BasePrice { get; set; }
    public int MinOrderQuantity { get; set; }

    public string ImageClass { get; set; } = "food-image-one";
    public bool IsAvailable { get; set; }
}