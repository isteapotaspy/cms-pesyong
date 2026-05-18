using CMS.Contracts.Admin.Orders;
using CMS.Contracts.Customer.Orders;
using CommunityToolkit.Mvvm.ComponentModel;

namespace PESYONG.Presentation.Admin.ViewModels;

public partial class OrderItemMealSelectionItemVM : ObservableObject
{
    [ObservableProperty]
    private int id;

    [ObservableProperty]
    private int orderItemId;

    [ObservableProperty]
    private int packageSelectionRuleId;

    [ObservableProperty]
    private int mealId;

    [ObservableProperty]
    private string ruleTitleSnapshot = string.Empty;

    [ObservableProperty]
    private string mealNameSnapshot = string.Empty;

    [ObservableProperty]
    private decimal additionalPrice;

    public static OrderItemMealSelectionItemVM FromDto(CMS.Contracts.Admin.Orders.OrderItemMealSelectionRequestDto dto)
    {
        return new OrderItemMealSelectionItemVM
        {
            Id = dto.Id ?? 0,
            OrderItemId = dto?.OrderItemId ?? 0,
            PackageSelectionRuleId = dto?.PackageSelectionRuleId ?? -1,
            MealId = dto?.MealId ?? -1,
            RuleTitleSnapshot = dto.RuleTitleSnapshot,
            MealNameSnapshot = dto.MealNameSnapshot,
            AdditionalPrice = dto.AdditionalPrice
        };
    }

    public void CopyFrom(OrderItemMealSelectionItemVM other)
    {
        Id = other.Id;
        OrderItemId = other.OrderItemId;
        PackageSelectionRuleId = other.PackageSelectionRuleId;
        MealId = other.MealId;
        RuleTitleSnapshot = other.RuleTitleSnapshot;
        MealNameSnapshot = other.MealNameSnapshot;
        AdditionalPrice = other.AdditionalPrice;
    }

    public OrderItemMealSelectionRequest ToRequest()
    {
        return new OrderItemMealSelectionRequest
        {
            Id = Id,
            PackageSelectionRuleId = PackageSelectionRuleId,
            MealId = MealId,
            RuleTitleSnapshot = RuleTitleSnapshot,
            MealNameSnapshot = MealNameSnapshot,
            AdditionalPrice = AdditionalPrice
        };
    }
}
