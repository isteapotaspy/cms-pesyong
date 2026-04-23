using Microsoft.AspNetCore.Components;
using MauiApp_Pesyong.Shared.Admin_Main.Admin_Models;
using MauiApp_Pesyong.Shared.Admin_Main.Admin_Services;

namespace MauiApp_Pesyong.Shared.Admin_Main.Admin_Pages;

public partial class CateringPackages : ComponentBase
{
    [Inject]
    public AdminDataService DataService { get; set; } = default!;

    protected List<PackageVm> packages = new();
    protected List<MealVm> meals = new();
    protected PackageVm selectedPackage = new();

    protected string newMealName = "";
    protected int newMealQuantity = 1;

    protected override void OnInitialized()
    {
        meals = DataService.GetMeals();
        packages = DataService.GetPackages();
        selectedPackage = ClonePackage(packages.FirstOrDefault() ?? new PackageVm());
    }

    protected decimal PackageTotal => selectedPackage.IncludedMeals.Sum(x => x.Price * x.Quantity);

    protected void SelectPackage(PackageVm package)
    {
        selectedPackage = ClonePackage(package);
    }

    protected void CreateNewPackage()
    {
        selectedPackage = new PackageVm
        {
            IsCateringPackage = true,
            IsAvailable = true,
            Pax = 20
        };
        newMealName = "";
        newMealQuantity = 1;
    }

    protected void AddMealToPackage()
    {
        if (string.IsNullOrWhiteSpace(newMealName))
            return;

        var sourceMeal = meals.FirstOrDefault(x => x.Title == newMealName);
        if (sourceMeal is null)
            return;

        selectedPackage.IncludedMeals.Add(new PackageMealVm
        {
            MealName = sourceMeal.Title,
            Price = sourceMeal.Price,
            Quantity = newMealQuantity <= 0 ? 1 : newMealQuantity,
            SpecialRequest = "Special request"
        });

        newMealName = "";
        newMealQuantity = 1;
    }

    protected void RemoveMeal(PackageMealVm meal)
    {
        selectedPackage.IncludedMeals.Remove(meal);
    }

    protected void SavePackage()
    {
        selectedPackage.Price = PackageTotal;

        var existing = packages.FirstOrDefault(x => x.PackageId == selectedPackage.PackageId);

        if (existing is null)
        {
            packages.Add(ClonePackage(selectedPackage));
        }
        else
        {
            existing.OwnerId = selectedPackage.OwnerId;
            existing.PromoId = selectedPackage.PromoId;
            existing.Pax = selectedPackage.Pax;
            existing.IsCateringPackage = selectedPackage.IsCateringPackage;
            existing.IsAvailable = selectedPackage.IsAvailable;
            existing.CustomizablePackage = selectedPackage.CustomizablePackage;
            existing.Name = selectedPackage.Name;
            existing.Description = selectedPackage.Description;
            existing.Notes = selectedPackage.Notes;
            existing.Price = PackageTotal;
            existing.IncludedMeals = selectedPackage.IncludedMeals
                .Select(x => new PackageMealVm
                {
                    MealName = x.MealName,
                    Price = x.Price,
                    Quantity = x.Quantity,
                    SpecialRequest = x.SpecialRequest
                }).ToList();
        }
    }

    private static PackageVm ClonePackage(PackageVm package) => new()
    {
        PackageId = package.PackageId,
        OwnerId = package.OwnerId,
        PromoId = package.PromoId,
        Pax = package.Pax,
        IsCateringPackage = package.IsCateringPackage,
        IsAvailable = package.IsAvailable,
        CustomizablePackage = package.CustomizablePackage,
        Name = package.Name,
        Description = package.Description,
        Notes = package.Notes,
        Price = package.Price,
        IncludedMeals = package.IncludedMeals.Select(x => new PackageMealVm
        {
            MealName = x.MealName,
            Price = x.Price,
            Quantity = x.Quantity,
            SpecialRequest = x.SpecialRequest   
        }).ToList()
    };
}
