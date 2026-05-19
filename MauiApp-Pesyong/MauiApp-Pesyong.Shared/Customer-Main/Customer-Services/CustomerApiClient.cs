using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Web;
using CMS.Contracts.Customer.Meals;
using CMS.Contracts.Customer.Menu;
using CMS.Contracts.Customer.Orders;
using MauiApp_Pesyong.Shared.Customer_Main.Customer_Models;
using MauiApp_Pesyong.Shared.Customer_Main.Customer_Vms;

namespace MauiApp_Pesyong.Shared.Customer_Main.Customer_Services;

public sealed class CustomerApiClient : ICustomerCatalogService, ICustomerOrderService
{
    private readonly HttpClient _http;
    private readonly ICustomerTokenStore _tokenStore;

    public CustomerApiClient(HttpClient http, ICustomerTokenStore tokenStore)
    {
        _http = http;
        _tokenStore = tokenStore;
    }

    public async Task<IReadOnlyList<string>> GetCategoriesAsync(CancellationToken cancellationToken = default)
    {
        var response = await _http.GetAsync("api/customer/menu", cancellationToken);

        if (!response.IsSuccessStatusCode)
            throw new InvalidOperationException(await ReadErrorMessageAsync(response));

        var dto = await response.Content.ReadFromJsonAsync<GetMenuResponse>(cancellationToken: cancellationToken)
                  ?? new GetMenuResponse();

        return dto.Categories
            .Select(x => x.Name)
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    public async Task<IReadOnlyList<PackageUiModel>> GetPackagesAsync(CancellationToken cancellationToken = default)
    {
        var response = await _http.GetAsync("api/customer/menu", cancellationToken);

        if (!response.IsSuccessStatusCode)
            throw new InvalidOperationException(await ReadErrorMessageAsync(response));

        var dto = await response.Content.ReadFromJsonAsync<GetMenuResponse>(cancellationToken: cancellationToken)
                  ?? new GetMenuResponse();

        return dto.Packages
            .Select(x => x.ToUiModel())
            .ToList();
    }

    public async Task<PackageUiModel?> GetPackageByIdAsync(string packageId, CancellationToken cancellationToken = default)
    {
        if (!int.TryParse(packageId, out var id) || id <= 0)
            return null;

        var response = await _http.GetAsync($"api/customer/packages/{id}", cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;

        if (!response.IsSuccessStatusCode)
            throw new InvalidOperationException(await ReadErrorMessageAsync(response));

        var dto = await response.Content.ReadFromJsonAsync<MenuPackageDto>(cancellationToken: cancellationToken);
        return dto?.ToUiModel();
    }

    public async Task<IReadOnlyList<ShortOrderMealUiModel>> GetMealsAsync(
    string? categorySlug = null,
    string? search = null,
    CancellationToken cancellationToken = default)
    {
        var url = BuildMealsUrl(categorySlug, search);

        using var response = await _http.GetAsync(url, cancellationToken);

        if (!response.IsSuccessStatusCode)
            throw new InvalidOperationException(await ReadErrorMessageAsync(response));

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        using var document = await JsonDocument.ParseAsync(stream, cancellationToken: cancellationToken);

        List<CustomerMealDto> dtoList = document.RootElement.ValueKind switch
        {
            JsonValueKind.Array => DeserializeMealArray(document.RootElement),
            JsonValueKind.Object => TryReadMealArrayProperty(document.RootElement, "meals")
                                    ?? TryReadMealArrayProperty(document.RootElement, "items")
                                    ?? TryReadMealArrayProperty(document.RootElement, "data")
                                    ?? throw new InvalidOperationException(
                                        "Unexpected meals response format. Expected an array or an object containing 'meals', 'items', or 'data'."),
            _ => throw new InvalidOperationException("Unexpected meals response format.")
        };

        return dtoList.Select(x => x.ToUiModel()).ToList();
    }

    public async Task<ShortOrderMealUiModel?> GetMealByIdAsync(
        int mealId,
        CancellationToken cancellationToken = default)
    {
        var response = await _http.GetAsync($"api/customer/meals/{mealId}", cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;

        if (!response.IsSuccessStatusCode)
            throw new InvalidOperationException(await ReadErrorMessageAsync(response));

        var dto = await response.Content.ReadFromJsonAsync<CustomerMealDto>(cancellationToken: cancellationToken);
        return dto?.ToUiModel();
    }

    public async Task<PlaceOrderResponse> PlaceOrderAsync(CheckoutVm vm, CancellationToken cancellationToken = default)
    {
        using var request = await CreateAuthorizedRequestAsync(HttpMethod.Post, "api/customer/orders");
        request.Content = JsonContent.Create(vm.ToRequest());

        using var response = await _http.SendAsync(request, cancellationToken);

        if (!response.IsSuccessStatusCode)
            throw new InvalidOperationException(await ReadErrorMessageAsync(response));

        return await response.Content.ReadFromJsonAsync<PlaceOrderResponse>(cancellationToken: cancellationToken)
               ?? throw new InvalidOperationException("Place order returned an empty response.");
    }

    public async Task<IReadOnlyList<CustomerOrderListItemDto>> GetMyOrdersAsync(CancellationToken cancellationToken = default)
    {
        using var request = await CreateAuthorizedRequestAsync(HttpMethod.Get, "api/customer/orders/my");
        using var response = await _http.SendAsync(request, cancellationToken);

        if (!response.IsSuccessStatusCode)
            throw new InvalidOperationException(await ReadErrorMessageAsync(response));

        var dto = await response.Content.ReadFromJsonAsync<List<CustomerOrderListItemDto>>(cancellationToken: cancellationToken)
                  ?? new List<CustomerOrderListItemDto>();

        return dto;
    }

    public async Task<TrackingVm?> GetTrackingAsync(int orderId, CancellationToken cancellationToken = default)
    {
        using var request = await CreateAuthorizedRequestAsync(HttpMethod.Get, $"api/customer/orders/{orderId}/tracking");
        using var response = await _http.SendAsync(request, cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;

        if (!response.IsSuccessStatusCode)
            throw new InvalidOperationException(await ReadErrorMessageAsync(response));

        var dto = await response.Content.ReadFromJsonAsync<GetOrderTrackingResponse>(cancellationToken: cancellationToken);
        return dto?.ToVm();
    }

    private static string BuildMealsUrl(string? categorySlug, string? search)
    {
        var query = HttpUtility.ParseQueryString(string.Empty);

        if (!string.IsNullOrWhiteSpace(categorySlug))
            query["categorySlug"] = categorySlug;

        if (!string.IsNullOrWhiteSpace(search))
            query["search"] = search;

        var qs = query.ToString();
        return string.IsNullOrWhiteSpace(qs)
            ? "api/customer/meals"
            : $"api/customer/meals?{qs}";
    }

    private async Task<HttpRequestMessage> CreateAuthorizedRequestAsync(HttpMethod method, string url)
    {
        var token = await _tokenStore.GetTokenAsync();

        if (string.IsNullOrWhiteSpace(token))
            throw new InvalidOperationException("You are not signed in.");

        var request = new HttpRequestMessage(method, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        return request;
    }

    private static async Task<string> ReadErrorMessageAsync(HttpResponseMessage response)
    {
        try
        {
            var raw = await response.Content.ReadAsStringAsync();

            if (string.IsNullOrWhiteSpace(raw))
                return $"Request failed with status code {(int)response.StatusCode} ({response.StatusCode}).";

            var parsed = JsonSerializer.Deserialize<ApiMessageResponse>(raw, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return !string.IsNullOrWhiteSpace(parsed?.Message)
                ? parsed.Message
                : raw;
        }
        catch
        {
            return $"Request failed with status code {(int)response.StatusCode} ({response.StatusCode}).";
        }
    }


    //==== HELPER CLASSES AND METHODS ====//
    private sealed class ApiMessageResponse
    {
        public string Message { get; set; } = string.Empty;
    }

    private static List<CustomerMealDto> DeserializeMealArray(JsonElement element)
    {
        return JsonSerializer.Deserialize<List<CustomerMealDto>>(
                   element.GetRawText(),
                   new JsonSerializerOptions
                   {
                       PropertyNameCaseInsensitive = true
                   })
               ?? new List<CustomerMealDto>();
    }

    private static List<CustomerMealDto>? TryReadMealArrayProperty(JsonElement root, string propertyName)
    {
        if (!root.TryGetProperty(propertyName, out var property))
            return null;

        if (property.ValueKind != JsonValueKind.Array)
            return null;

        return DeserializeMealArray(property);
    }
}