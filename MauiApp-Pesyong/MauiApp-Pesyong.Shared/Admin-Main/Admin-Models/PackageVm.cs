namespace MauiApp_Pesyong.Shared.Admin_Main.Admin_Models;

public class PackageVm
{
    public string PackageId { get; set; } = "";
    public string OwnerId { get; set; } = "";
    public string PromoId { get; set; } = "";
    public int Pax { get; set; }
    public bool IsCateringPackage { get; set; } = true;
    public bool IsAvailable { get; set; } = true;
    public bool CustomizablePackage { get; set; }
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public string Notes { get; set; } = "";
    public decimal Price { get; set; }
    public List<PackageMealVm> IncludedMeals { get; set; } = new();
}

public class PackageMealVm
{
    public string MealName { get; set; } = "";
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public string SpecialRequest { get; set; } = "";
}
