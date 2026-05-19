using CMS.Contracts.Customer.Meals;
using CMS.Contracts.Customer.Menu;
using CMS.Contracts.Customer.Orders;
using CMS.Contracts.Customer.Promos;
using MauiApp_Pesyong.Shared.Customer_Main.Customer_Models;
using MauiApp_Pesyong.Shared.Customer_Main.Customer_Vms;
using System.Globalization;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Web;
using static System.Net.WebRequestMethods;

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

    public async Task<DownloadedFileVm?> DownloadInvoiceAsync(int orderId, CancellationToken cancellationToken = default)
    {
        using var request = await CreateAuthorizedRequestAsync(HttpMethod.Get, $"api/customer/orders/{orderId}/invoice");
        using var response = await _http.SendAsync(request, cancellationToken);

        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            return null;

        if (!response.IsSuccessStatusCode)
            throw new InvalidOperationException(await ReadErrorMessageAsync(response));

        var bytes = await response.Content.ReadAsByteArrayAsync(cancellationToken);

        var fileName =
            response.Content.Headers.ContentDisposition?.FileNameStar ??
            response.Content.Headers.ContentDisposition?.FileName?.Trim('"') ??
            $"invoice-{orderId}.html";

        var contentType =
            response.Content.Headers.ContentType?.MediaType ??
            "text/html";

        return new DownloadedFileVm
        {
            FileName = fileName,
            ContentType = contentType,
            Content = bytes
        };
    }

    public async Task<PromoValidationResponse?> ValidatePromoAsync(
    string code,
    decimal subTotal,
    CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(code))
            return null;

        var url =
            $"api/customer/promos/validate?code={Uri.EscapeDataString(code)}&subTotal={subTotal}";

        using var response = await _http.GetAsync(url, cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(cancellationToken);

            throw new InvalidOperationException(
                $"Promo validation failed. Status: {(int)response.StatusCode} {response.ReasonPhrase}. Body: {body}");
        }

        return await response.Content.ReadFromJsonAsync<PromoValidationResponse>(
            cancellationToken);
    }
    public async Task<IReadOnlyList<ActivePromoDto>> GetActivePromosAsync(
    CancellationToken cancellationToken = default)
    {
        using var response = await _http.GetAsync(
            "api/customer/promos/active",
            cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return new List<ActivePromoDto>();
        }

        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(cancellationToken);

            throw new InvalidOperationException(
                $"Loading active promos failed. Status: {(int)response.StatusCode} {response.ReasonPhrase}. Body: {body}");
        }

        var items = await response.Content.ReadFromJsonAsync<List<ActivePromoDto>>(
            cancellationToken: cancellationToken);

        return items ?? new List<ActivePromoDto>();
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

    private static async Task<string> ReadErrorMessageAsync(
    HttpResponseMessage response,
    CancellationToken cancellationToken = default)
    {
        var body = await response.Content.ReadAsStringAsync(cancellationToken);

        if (string.IsNullOrWhiteSpace(body))
        {
            return $"Request failed with status code {(int)response.StatusCode} ({response.StatusCode}).";
        }

        // If ASP.NET returns ProblemDetails JSON
        try
        {
            using var document = JsonDocument.Parse(body);
            var root = document.RootElement;

            if (root.TryGetProperty("message", out var message))
                return message.GetString() ?? "Request failed.";

            if (root.TryGetProperty("title", out var title))
                return title.GetString() ?? "Request failed.";

            if (root.TryGetProperty("detail", out var detail))
                return detail.GetString() ?? "Request failed.";
        }
        catch
        {
            // Body is probably plain text or HTML, not JSON.
        }

        
        if (body.TrimStart().StartsWith("<"))
        {
            return $"Server error: {(int)response.StatusCode} ({response.StatusCode}). Check the ASP.NET backend console/output window.";
        }

        return body;
    }
}