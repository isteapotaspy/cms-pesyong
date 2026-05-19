using CMS.Contracts.Admin.Orders;
using CMS.Contracts.Customer.Orders;
using CMS.Domain.Entities.Orders;
using CMS.Domain.Entities.Packages;
using CommunityToolkit.Mvvm.ComponentModel;

namespace PESYONG.Presentation.Admin.ViewModels.Orders;

public partial class OrderItemAddonSelectionItemVM : ObservableObject
{
    [ObservableProperty]
    private int id;

    [ObservableProperty]
    private int orderItemId;

    [ObservableProperty]
    private int packageAddonId;

    [ObservableProperty]
    private string addonNameSnapshot = string.Empty;

    [ObservableProperty]
    private decimal additionalPrice;

    public static OrderItemAddonSelectionItemVM FromDto(OrderItemAddonSelectionDto dto)
    {
        return new OrderItemAddonSelectionItemVM
        {
            Id = dto.Id,
            OrderItemId = dto.OrderItemId,
            PackageAddonId = dto.PackageAddonId,
            AddonNameSnapshot = dto.AddonNameSnapshot,
            AdditionalPrice = dto.AdditionalPrice
        };
    }

    public void CopyFrom(OrderItemAddonSelectionItemVM other)
    {
        Id = other.Id;
        OrderItemId = other.OrderItemId;
        PackageAddonId = other.PackageAddonId;
        AddonNameSnapshot = other.AddonNameSnapshot;
        AdditionalPrice = other.AdditionalPrice;
    }

    public OrderItemAddonSelectionRequest ToRequest()
    {
        return new OrderItemAddonSelectionRequest
        {
            Id = Id,
            PackageAddonId = PackageAddonId,
            AddonNameSnapshot = AddonNameSnapshot,
            AdditionalPrice = AdditionalPrice
        };
    }
}