using MauiApp_Pesyong.Shared.Customer_Main.Customer_Models;
using MauiApp_Pesyong.Shared.Customer_Main.Customer_Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace MauiApp_Pesyong.Shared.Customer_Main.Customer_Vms;
public class ShortOrdersVm
{
    private readonly ICustomerCatalogService _catalogService;

    public ShortOrdersVm(ICustomerCatalogService catalogService)
    {
        _catalogService = catalogService;
    }

    public string SearchText { get; set; } = string.Empty;
    public bool IsLoading { get; private set; }
    public string? ErrorMessage { get; private set; }

    public List<ShortOrderMealUiModel> Meals { get; private set; } = new();

    public async Task LoadAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            IsLoading = true;
            ErrorMessage = null;

            Meals = (await _catalogService.GetMealsAsync(
                categorySlug: "short-orders",
                search: string.IsNullOrWhiteSpace(SearchText) ? null : SearchText,
                cancellationToken: cancellationToken))
                .ToList();
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
            Meals = new List<ShortOrderMealUiModel>();
        }
        finally
        {
            IsLoading = false;
        }
    }
}
