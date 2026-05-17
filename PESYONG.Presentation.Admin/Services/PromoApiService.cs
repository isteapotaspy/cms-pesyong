using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using CMS.Contracts.Admin.Promos;
using PESYONG.Presentation.Admin.Interfaces;

namespace PESYONG.Presentation.Admin.Services;

public sealed class PromoApiService : IPromoApiService
{
    private const string Route = "api/admin/promos";

    private readonly HttpClient _httpClient;

    public PromoApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IReadOnlyList<PromoDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var promos = await _httpClient.GetFromJsonAsync<List<PromoDto>>(Route, cancellationToken);
        return promos ?? new List<PromoDto>();
    }

    public async Task<PromoDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        using var response = await _httpClient.GetAsync($"{Route}/{id}", cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<PromoDto>(cancellationToken);
    }

    public async Task<PromoDto> CreateAsync(CreatePromoRequest request, CancellationToken cancellationToken = default)
    {
        using var response = await _httpClient.PostAsJsonAsync(Route, request, cancellationToken);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<PromoDto>(cancellationToken)
               ?? throw new InvalidOperationException("The API returned an empty promo response.");
    }

    public async Task<PromoDto> UpdateAsync(int id, UpdatePromoRequest request, CancellationToken cancellationToken = default)
    {
        using var response = await _httpClient.PutAsJsonAsync($"{Route}/{id}", request, cancellationToken);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<PromoDto>(cancellationToken)
               ?? throw new InvalidOperationException("The API returned an empty promo response.");
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        using var response = await _httpClient.DeleteAsync($"{Route}/{id}", cancellationToken);

        response.EnsureSuccessStatusCode();
    }
}
