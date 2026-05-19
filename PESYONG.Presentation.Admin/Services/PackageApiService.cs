using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using CMS.Contracts.Admin.Package;
using PESYONG.Presentation.Admin.Interfaces;

namespace PESYONG.Presentation.Admin.Services;

public sealed class PackageApiService : IPackageApiService
{
    private readonly HttpClient _httpClient;

    public PackageApiService(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("CMSApi");
    }

    public async Task<List<PackageDto>> GetAllAsync()
    {
        var result = await _httpClient.GetFromJsonAsync<List<PackageDto>>("api/admin/packages");
        return result ?? new List<PackageDto>();
    }

    public async Task<PackageDto?> GetByIdAsync(int id)
    {
        var response = await _httpClient.GetAsync($"api/admin/packages/{id}");

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        await EnsureSuccessWithBodyAsync(response, "Get package");

        return await response.Content.ReadFromJsonAsync<PackageDto>();
    }

    public async Task<PackageLookupDto> GetLookupsAsync()
    {
        var result = await _httpClient.GetFromJsonAsync<PackageLookupDto>("api/admin/packages/lookups");
        return result ?? new PackageLookupDto();
    }

    public async Task<PackageDto> CreateAsync(CreatePackageRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync("api/admin/packages", request);

        await EnsureSuccessWithBodyAsync(response, "Create package");

        return await response.Content.ReadFromJsonAsync<PackageDto>()
               ?? throw new InvalidOperationException("The API returned an empty package response.");
    }

    public async Task<PackageDto> UpdateAsync(int id, UpdatePackageRequest request)
    {
        var response = await _httpClient.PutAsJsonAsync($"api/admin/packages/{id}", request);

        await EnsureSuccessWithBodyAsync(response, "Update package");

        return await response.Content.ReadFromJsonAsync<PackageDto>()
               ?? throw new InvalidOperationException("The API returned an empty package response.");
    }

    public async Task DeleteAsync(int id)
    {
        var response = await _httpClient.DeleteAsync($"api/admin/packages/{id}");

        await EnsureSuccessWithBodyAsync(response, "Delete package");
    }

    private static async Task EnsureSuccessWithBodyAsync(
        HttpResponseMessage response,
        string operationName)
    {
        if (response.IsSuccessStatusCode)
        {
            return;
        }

        var body = await response.Content.ReadAsStringAsync();

        throw new HttpRequestException(
            $"{operationName} failed. Status: {(int)response.StatusCode} {response.ReasonPhrase}. Body: {body}");
    }
}