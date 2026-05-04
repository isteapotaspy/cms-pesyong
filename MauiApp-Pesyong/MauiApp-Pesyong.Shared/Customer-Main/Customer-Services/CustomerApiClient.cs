
using CMS.Contracts.Customer.Menu;
using CMS.Contracts.Customer.Orders;
using MauiApp_Pesyong.Shared.Customer_Main.Customer_Models;
using MauiApp_Pesyong.Shared.Customer_Main.Customer_Vms;
using System.Net.Http.Json;

namespace MauiApp_Pesyong.Shared.Customer_Main.Customer_Services;

public class CustomerApiClient : ICustomerCatalogService, ICustomerOrderService
{
    private readonly HttpClient _http;

    public CustomerApiClient(HttpClient http)
    {
        _http = http;
    }

    public async Task<IReadOnlyList<string>> GetCategoriesAsync(CancellationToken cancellationToken = default)
    {
        var response = await _http.GetFromJsonAsync<GetMenuResponse>("api/customer/menu", cancellationToken);
        return response?.Categories.Select(x => x.Name).ToList() ?? new List<string>();
    }

    public async Task<IReadOnlyList<PackageUiModel>> GetPackagesAsync(CancellationToken cancellationToken = default)
    {
        var response = await _http.GetFromJsonAsync<GetMenuResponse>("api/customer/menu", cancellationToken);

        if (response is null)
            return Array.Empty<PackageUiModel>();

        return response.Packages.Select(x => x.ToUiModel()).ToList();
    }

    public async Task<PackageUiModel?> GetPackageByIdAsync(string packageId, CancellationToken cancellationToken = default)
    {
        var dto = await _http.GetFromJsonAsync<MenuPackageDto>($"api/customer/packages/{packageId}", cancellationToken);
        return dto?.ToUiModel();
    }

    public async Task<PlaceOrderResponse> PlaceOrderAsync(CheckoutVm vm, CancellationToken cancellationToken = default)
    {
        var request = vm.ToRequest();

        var response = await _http.PostAsJsonAsync("api/customer/orders", request, cancellationToken);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<PlaceOrderResponse>(cancellationToken: cancellationToken);
        return result ?? throw new InvalidOperationException("Order API returned an empty response.");
    }

    public async Task<TrackingVm?> GetTrackingAsync(string orderId, CancellationToken cancellationToken = default)
    {
        var dto = await _http.GetFromJsonAsync<GetOrderTrackingResponse>($"api/customer/orders/{orderId}/tracking", cancellationToken);
        return dto?.ToVm();
    }
}