using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using CMS.Contracts.Admin.Address;
using PESYONG.Presentation.Admin.Interfaces;

namespace PESYONG.Presentation.Admin.Services;

public sealed class AdminCustomerAddressApiService : IAdminCustomerAddressApiService
{
    private readonly HttpClient _httpClient;

    public AdminCustomerAddressApiService(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient("CMSApi");
    }

    public async Task<CustomerAddressStringDto?> GetCustomerAddressAsync(
        int customerProfileId,
        int addressId,
        CancellationToken cancellationToken = default)
    {
        var url = $"api/admin/customer-addresses/customer/{customerProfileId}/address/{addressId}";

        using var response = await _httpClient.GetAsync(url, cancellationToken);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync(cancellationToken);

            throw new InvalidOperationException(
                $"Customer/address lookup failed. Status: {(int)response.StatusCode} {response.ReasonPhrase}. Body: {errorBody}");
        }

        return await response.Content.ReadFromJsonAsync<CustomerAddressStringDto>(
            cancellationToken: cancellationToken);
    }
}