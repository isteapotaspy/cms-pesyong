using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using CMS.Contracts.Admin.Payment;
using PESYONG.Presentation.Admin.Interfaces;

namespace PESYONG.Presentation.Admin.Services;

public sealed class PaymentApiService : IPaymentApiService
{
    private const string Route = "api/admin/payments";

    private readonly HttpClient _httpClient;

    public PaymentApiService(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("CMSApi");
    }

    public async Task<IReadOnlyList<PaymentDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var payments = await _httpClient.GetFromJsonAsync<List<PaymentDto>>(Route, cancellationToken);
        return payments ?? [];
    }

    public async Task<PaymentDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync($"{Route}/{id}", cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<PaymentDto>(cancellationToken);
    }

    public async Task<PaymentDto> CreateAsync(CreatePaymentRequest request, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PostAsJsonAsync(Route, request, cancellationToken);
        response.EnsureSuccessStatusCode();

        var created = await response.Content.ReadFromJsonAsync<PaymentDto>(cancellationToken);

        return created ?? throw new InvalidOperationException("The API returned an empty payment response.");
    }

    public async Task<PaymentDto> UpdateAsync(int id, UpdatePaymentRequest request, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.PutAsJsonAsync($"{Route}/{id}", request, cancellationToken);
        response.EnsureSuccessStatusCode();

        var updated = await response.Content.ReadFromJsonAsync<PaymentDto>(cancellationToken);

        return updated ?? throw new InvalidOperationException("The API returned an empty payment response.");
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.DeleteAsync($"{Route}/{id}", cancellationToken);
        response.EnsureSuccessStatusCode();
    }
}
