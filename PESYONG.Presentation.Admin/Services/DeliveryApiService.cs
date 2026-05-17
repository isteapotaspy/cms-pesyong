using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using CMS.Contracts.Admin.Deliveries;
using PESYONG.Presentation.Admin.Interfaces;

namespace PESYONG.Presentation.Admin.Services;

public class DeliveryApiService : IDeliveryApiService
{
    private readonly HttpClient _httpClient;

    public DeliveryApiService(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("CMSApi");
    }

    public async Task<IReadOnlyList<DeliveryDto>> GetAllAsync()
    {
        var result = await _httpClient.GetFromJsonAsync<List<DeliveryDto>>("api/admin/deliveries");
        return result ?? new List<DeliveryDto>();
    }

    public async Task<DeliveryDto?> GetByIdAsync(int id)
    {
        var response = await _httpClient.GetAsync($"api/admin/deliveries/{id}");

        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<DeliveryDto>();
    }

    public async Task<DeliveryDto> CreateAsync(CreateDeliveryRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("api/admin/deliveries", request);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<DeliveryDto>()
               ?? throw new InvalidOperationException("The API returned an empty delivery response.");
    }

    public async Task<DeliveryDto> UpdateAsync(int id, UpdateDeliveryRequest request)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/admin/deliveries/{id}", request);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<DeliveryDto>()
               ?? throw new InvalidOperationException("The API returned an empty delivery response.");
    }

    public async Task DeleteAsync(int id)
    {
        var response = await _httpClient.DeleteAsync($"api/admin/deliveries/{id}");
        response.EnsureSuccessStatusCode();
    }
}