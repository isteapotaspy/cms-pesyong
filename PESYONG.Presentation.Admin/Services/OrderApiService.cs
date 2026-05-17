using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using CMS.Contracts.Admin.Orders;
using PESYONG.Presentation.Admin.Interfaces;

namespace PESYONG.Presentation.Admin.Services;

public sealed class OrderApiService : IOrderApiService
{
    private const string Route = "api/admin/orders";

    private readonly HttpClient _httpClient;

    public OrderApiService(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("CMSApi");
    }

    public async Task<List<OrderDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var result = await _httpClient.GetFromJsonAsync<List<OrderDto>>(Route, cancellationToken);
        return result ?? new List<OrderDto>();
    }

    public async Task<OrderDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync($"{Route}/{id}", cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<OrderDto>(cancellationToken);
    }

    public async Task<OrderDto> CreateAsync(CreateOrderRequest request, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsJsonAsync(Route, request, cancellationToken);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<OrderDto>(cancellationToken)
               ?? throw new InvalidOperationException("The API returned an empty order response.");
    }

    public async Task<OrderDto> UpdateAsync(int id, UpdateOrderRequest request, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PutAsJsonAsync($"{Route}/{id}", request, cancellationToken);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<OrderDto>(cancellationToken)
               ?? throw new InvalidOperationException("The API returned an empty order response.");
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.DeleteAsync($"{Route}/{id}", cancellationToken);
        response.EnsureSuccessStatusCode();
    }
}