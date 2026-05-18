using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using CMS.Contracts.Admin.Package;
using PESYONG.Presentation.Admin.Interfaces;

namespace PESYONG.Presentation.Admin.Services;

public sealed class PackageApiService : IPackageApiService
{
    private const string Route = "api/admin/packages";

    private readonly HttpClient _httpClient;

    public PackageApiService(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("CMSApi");
    }

    public async Task<List<PackageDto>> GetAllAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<PackageDto>>(Route)
               ?? new List<PackageDto>();
    }

    public async Task<PackageDto?> GetByIdAsync(int id)
    {
        var response = await _httpClient.GetAsync($"{Route}/{id}");

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<PackageDto>();
    }

    public async Task<PackageLookupDto> GetLookupsAsync()
    {
        return await _httpClient.GetFromJsonAsync<PackageLookupDto>($"{Route}/lookups")
               ?? new PackageLookupDto();
    }

    public async Task<PackageDto> CreateAsync(CreatePackageRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync(Route, request);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<PackageDto>()
               ?? throw new InvalidOperationException("The API returned an empty package response.");
    }

    public async Task<PackageDto> UpdateAsync(int id, UpdatePackageRequest request)
    {
        var response = await _httpClient.PutAsJsonAsync($"{Route}/{id}", request);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<PackageDto>()
               ?? throw new InvalidOperationException("The API returned an empty package response.");
    }

    public async Task DeleteAsync(int id)
    {
        var response = await _httpClient.DeleteAsync($"{Route}/{id}");
        response.EnsureSuccessStatusCode();
    }
}