using CMS.Contracts.Customer.Orders;
using MauiApp_Pesyong.Shared.Customer_Main.Customer_Models;

namespace MauiApp_Pesyong.Shared.Services;

public class CustomerDrawerState
{
    public event Action? OnChange;

    public bool IsPackageDrawerOpen { get; private set; }
    public bool IsCartDrawerOpen { get; private set; }

    public PackageUiModel? SelectedPackage { get; private set; }
    public List<CartLineUiModel> CartItems { get; } = new();

    public int? CurrentOrderId { get; private set; }
    public string? CurrentOrderNumber { get; private set; }

    public int CartCount => CartItems.Sum(x => x.Quantity);
    public decimal SubTotal => CartItems.Sum(x => x.LineTotal);

    public void OpenPackage(PackageUiModel packageItem)
    {
        SelectedPackage = packageItem;
        IsCartDrawerOpen = false;
        IsPackageDrawerOpen = true;
        NotifyStateChanged();
    }

    public void OpenCart()
    {
        IsPackageDrawerOpen = false;
        IsCartDrawerOpen = true;
        NotifyStateChanged();
    }

    public void SetPackageDrawer(bool open)
    {
        IsPackageDrawerOpen = open;
        if (open)
            IsCartDrawerOpen = false;

        NotifyStateChanged();
    }

    public void SetCartDrawer(bool open)
    {
        IsCartDrawerOpen = open;
        if (open)
            IsPackageDrawerOpen = false;

        NotifyStateChanged();
    }

    public void CloseAll()
    {
        IsPackageDrawerOpen = false;
        IsCartDrawerOpen = false;
        NotifyStateChanged();
    }

    public void AddToCart(CartLineUiModel line)
    {
        var existing = CartItems.FirstOrDefault(x =>
            x.IsMealItem == line.IsMealItem &&
            (
                (x.IsMealItem && x.MealId == line.MealId) ||
                (!x.IsMealItem &&
                 x.PackageId == line.PackageId &&
                 x.PackageSizeId == line.PackageSizeId &&
                 x.Notes == line.Notes)
            ));

        if (existing is null)
        {
            CartItems.Add(line);
        }
        else
        {
            existing.Quantity += line.Quantity;
        }

        IsPackageDrawerOpen = false;
        IsCartDrawerOpen = true;
        NotifyStateChanged();
    }

    public void IncreaseItem(CartLineUiModel item)
    {
        item.Quantity++;
        NotifyStateChanged();
    }

    public void DecreaseItem(CartLineUiModel item)
    {
        if (item.Quantity > 1)
        {
            item.Quantity--;
            NotifyStateChanged();
        }
    }

    public void RemoveItem(CartLineUiModel item)
    {
        if (CartItems.Remove(item))
            NotifyStateChanged();
    }

    public void SetPlacedOrder(PlaceOrderResponse response)
    {
        CurrentOrderId = response.OrderId;
        CurrentOrderNumber = response.OrderNumber;
        NotifyStateChanged();
    }

    public void ClearCartAfterCheckout()
    {
        CartItems.Clear();
        IsCartDrawerOpen = false;
        IsPackageDrawerOpen = false;
        NotifyStateChanged();
    }

    public void NotifyStateChanged()
    {
        OnChange?.Invoke();
    }
}