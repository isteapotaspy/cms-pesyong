using Microsoft.AspNetCore.Components;
using MauiApp_Pesyong.Shared.Admin_Main.Admin_Models;
using MauiApp_Pesyong.Shared.Admin_Main.Admin_Services;

namespace MauiApp_Pesyong.Shared.Admin_Main.Admin_Pages;

public partial class KioskMeals : ComponentBase
{
    [Inject]
    public AdminDataService DataService { get; set; } = default!;

    protected string searchText = "";
    protected List<MealVm> meals = new();
    protected MealVm selectedMeal = new();
    protected string creatorId = "";
    protected DateTime mealCreatedDate = DateTime.Today;

    protected override void OnInitialized()
    {
        // Be defensive if DataService or returned list is null
        var fetched = DataService?.GetMeals();
        meals = fetched ?? new List<MealVm>();
        selectedMeal = meals.FirstOrDefault() is MealVm first ? CloneMeal(first) : new MealVm();
    }

    // Make the filter null-safe: handle null collections and null title/description
    protected IEnumerable<MealVm> FilteredMeals =>
        meals.Where(x =>
            string.IsNullOrWhiteSpace(searchText) ||
            x.Title.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
            x.Description.Contains(searchText, StringComparison.OrdinalIgnoreCase));

    protected void SelectMeal(MealVm meal)
    {
        selectedMeal = CloneMeal(meal);
    }

    protected void ClearForm()
    {
        selectedMeal = new MealVm
        {
            DeliveryType = "Delivery"
        };
        creatorId = "";
        mealCreatedDate = DateTime.Today;
    }

    protected void SaveMeal()
    {
        var existing = meals.FirstOrDefault(x => x.MealId == selectedMeal.MealId);

        if (existing is null)
        {
            meals.Add(CloneMeal(selectedMeal));
        }
        else
        {
            existing.Title = selectedMeal.Title;
            existing.Description = selectedMeal.Description;
            existing.Price = selectedMeal.Price;
            existing.DeliveryType = selectedMeal.DeliveryType;
            existing.StockQuantity = selectedMeal.StockQuantity;
            existing.MinOrder = selectedMeal.MinOrder;
        }
    }

    protected void DeleteMeal()
    {
        var existing = meals.FirstOrDefault(x => x.MealId == selectedMeal.MealId);
        if (existing is not null)
        {
            meals.Remove(existing);
            ClearForm();
        }
    }

    private static MealVm CloneMeal(MealVm meal) => new()
    {
        MealId = meal.MealId,
        Title = meal.Title,
        Description = meal.Description,
        Price = meal.Price,
        DeliveryType = meal.DeliveryType,
        StockQuantity = meal.StockQuantity,
        MinOrder = meal.MinOrder
    };
}
