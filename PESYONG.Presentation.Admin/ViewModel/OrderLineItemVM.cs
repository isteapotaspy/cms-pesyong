using System.Collections.ObjectModel;
using CMS.Contracts.Admin.Orders;
using CMS.Domain.Entities.Menu;
using CMS.Domain.Entities.Orders;
using CMS.Domain.Entities.Packages;
using CommunityToolkit.Mvvm.ComponentModel;

namespace PESYONG.Presentation.Admin.ViewModels.Orders;

public partial class OrderLineItemVM : ObservableObject
{
    [ObservableProperty]
    private int id;

    [ObservableProperty]
    private int orderId;

    [ObservableProperty]
    private string itemType = "Package";

    [ObservableProperty]
    private int? packageId;

    [ObservableProperty]
    private int? packageSizeId;

    [ObservableProperty]
    private int? mealId;

    [ObservableProperty]
    private string packageTitleSnapshot = string.Empty;

    [ObservableProperty]
    private string sizeLabelSnapshot = string.Empty;

    [ObservableProperty]
    private decimal baseUnitPrice;

    [ObservableProperty]
    private int quantity = 1;

    public ObservableCollection<OrderItemMealSelectionItemVM> MealSelections { get; } = new();
    public ObservableCollection<OrderItemAddonSelectionItemVM> AddonSelections { get; } = new();

    public decimal UnitPrice =>
        BaseUnitPrice
        + MealSelections.Sum(x => x.AdditionalPrice)
        + AddonSelections.Sum(x => x.AdditionalPrice);

    public decimal LineTotal => UnitPrice * Quantity;

    public OrderLineItemVM()
    {
        MealSelections.CollectionChanged += (_, _) => RefreshTotals();
        AddonSelections.CollectionChanged += (_, _) => RefreshTotals();
    }

    partial void OnBaseUnitPriceChanged(decimal value)
    {
        RefreshTotals();
    }

    partial void OnQuantityChanged(int value)
    {
        RefreshTotals();
    }

    private void RefreshTotals()
    {
        OnPropertyChanged(nameof(UnitPrice));
        OnPropertyChanged(nameof(LineTotal));
    }

    public static OrderLineItemVM FromDto(OrderLineItemDto dto)
    {
        var vm = new OrderLineItemVM
        {
            Id = dto.Id,
            OrderId = dto.OrderId,
            ItemType = dto.ItemType,
            PackageId = dto.PackageId,
            PackageSizeId = dto.PackageSizeId,
            MealId = dto.MealId,
            PackageTitleSnapshot = dto.PackageTitleSnapshot,
            SizeLabelSnapshot = dto.SizeLabelSnapshot,
            BaseUnitPrice = dto.BaseUnitPrice,
            Quantity = dto.Quantity
        };

        for (int i = 0; i < dto.MealSelections.Count; i++)
        {
            OrderItemMealSelectionRequestDto? mealSelection = dto.MealSelections[i];
            vm.MealSelections.Add(OrderItemMealSelectionItemVM.FromDto(mealSelection));
        }

        foreach (var addonSelection in dto.AddonSelections)
            vm.AddonSelections.Add(OrderItemAddonSelectionItemVM.FromDto(addonSelection));

        return vm;
    }

    public void CopyFrom(OrderLineItemVM other)
    {
        Id = other.Id;
        OrderId = other.OrderId;
        ItemType = other.ItemType;
        PackageId = other.PackageId;
        PackageSizeId = other.PackageSizeId;
        MealId = other.MealId;
        PackageTitleSnapshot = other.PackageTitleSnapshot;
        SizeLabelSnapshot = other.SizeLabelSnapshot;
        BaseUnitPrice = other.BaseUnitPrice;
        Quantity = other.Quantity;

        MealSelections.Clear();
        foreach (var mealSelection in other.MealSelections)
        {
            var copy = new OrderItemMealSelectionItemVM();
            copy.CopyFrom(mealSelection);
            MealSelections.Add(copy);
        }

        AddonSelections.Clear();
        foreach (var addonSelection in other.AddonSelections)
        {
            var copy = new OrderItemAddonSelectionItemVM();
            copy.CopyFrom(addonSelection);
            AddonSelections.Add(copy);
        }

        RefreshTotals();
    }

    public OrderLineItemRequest ToRequest()
    {
        return new OrderLineItemRequest
        {
            Id = Id,
            ItemType = ItemType,
            PackageId = PackageId,
            PackageSizeId = PackageSizeId,
            MealId = MealId,
            PackageTitleSnapshot = PackageTitleSnapshot,
            SizeLabelSnapshot = SizeLabelSnapshot,
            BaseUnitPrice = BaseUnitPrice,
            Quantity = Quantity,
            MealSelections = MealSelections.Select(x => x.ToRequest()).ToList(),
            AddonSelections = AddonSelections.Select(x => x.ToRequest()).ToList()
        };
    }
}