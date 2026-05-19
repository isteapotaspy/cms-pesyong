using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using CMS.Contracts.Customer.Auth;
using CMS.Contracts.Customer.Profile;

namespace MauiApp_Pesyong.Shared.Customer_Main.Customer_Services;

public class CustomerAuthService : ICustomerAuthService
{
    private readonly HttpClient _http;
    private readonly ICustomerTokenStore _tokenStore;
    private readonly CustomerSession _session;

    public CustomerAuthService(
        HttpClient http,
        ICustomerTokenStore tokenStore,
        CustomerSession session)
    {
        _http = http;
        _tokenStore = tokenStore;
        _session = session;
    }

    public async Task<RegisterResponse> RegisterAsync(
        CustomerRegisterRequest request,
        CancellationToken cancellationToken = default)
    {
        var response = await _http.PostAsJsonAsync(
            "api/customer/auth/register",
            request,
            cancellationToken);

        if (!response.IsSuccessStatusCode)
            throw new InvalidOperationException(await ReadErrorMessageAsync(response));

        var result = await response.Content.ReadFromJsonAsync<RegisterResponse>(cancellationToken: cancellationToken)
                     ?? throw new InvalidOperationException("Register returned an empty response.");

        return result;
    }

    public async Task<AuthResponse> VerifyEmailAsync(
        VerifyEmailRequest request,
        CancellationToken cancellationToken = default)
    {
        var response = await _http.PostAsJsonAsync(
            "api/customer/auth/verify-email",
            request,
            cancellationToken);

        if (!response.IsSuccessStatusCode)
            throw new InvalidOperationException(await ReadErrorMessageAsync(response));

        var result = await response.Content.ReadFromJsonAsync<AuthResponse>(cancellationToken: cancellationToken)
                     ?? throw new InvalidOperationException("Verify email returned an empty response.");

        await _tokenStore.SetTokenAsync(result.Token);
        _session.SetAuthenticated(result, result.Token);

        var me = await GetMeAsync(cancellationToken);
        if (me is not null)
            _session.SetProfile(me);

        return result;
    }

    public async Task<string> ResendVerificationCodeAsync(
        ResendVerificationCodeRequest request,
        CancellationToken cancellationToken = default)
    {
        var response = await _http.PostAsJsonAsync(
            "api/customer/auth/resend-code",
            request,
            cancellationToken);

        if (!response.IsSuccessStatusCode)
            throw new InvalidOperationException(await ReadErrorMessageAsync(response));

        var result = await response.Content.ReadFromJsonAsync<MessageResponse>(cancellationToken: cancellationToken);
        return result?.Message ?? "A new verification code was sent.";
    }

    public async Task<AuthResponse> LoginAsync(
        CustomerLoginRequest request,
        CancellationToken cancellationToken = default)
    {
        var response = await _http.PostAsJsonAsync(
            "api/customer/auth/login",
            request,
            cancellationToken);

        if (!response.IsSuccessStatusCode)
            throw new InvalidOperationException(await ReadErrorMessageAsync(response));

        var result = await response.Content.ReadFromJsonAsync<AuthResponse>(cancellationToken: cancellationToken)
                     ?? throw new InvalidOperationException("Login returned an empty response.");

        await _tokenStore.SetTokenAsync(result.Token);
        _session.SetAuthenticated(result, result.Token);

        var me = await GetMeAsync(cancellationToken);
        if (me is not null)
            _session.SetProfile(me);

        return result;
    }

    public async Task<CustomerMeResponse?> GetMeAsync(CancellationToken cancellationToken = default)
    {
        using var request = await CreateAuthorizedRequestAsync(HttpMethod.Get, "api/customer/auth/me");
        using var response = await _http.SendAsync(request, cancellationToken);

        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<CustomerMeResponse>(cancellationToken: cancellationToken);
    }

    public async Task<CustomerMeResponse> UpdateProfileAsync(
        UpdateCustomerProfileRequest request,
        CancellationToken cancellationToken = default)
    {
        using var httpRequest = await CreateAuthorizedRequestAsync(HttpMethod.Put, "api/customer/profile");
        httpRequest.Content = JsonContent.Create(request);

        using var response = await _http.SendAsync(httpRequest, cancellationToken);

        if (!response.IsSuccessStatusCode)
            throw new InvalidOperationException(await ReadErrorMessageAsync(response));

        var result = await response.Content.ReadFromJsonAsync<CustomerMeResponse>(cancellationToken: cancellationToken)
                     ?? throw new InvalidOperationException("Update profile returned an empty response.");

        _session.SetProfile(result);
        return result;
    }

    public async Task<IReadOnlyList<CustomerAddressDto>> GetSavedAddressesAsync(CancellationToken cancellationToken = default)
    {
        using var request = await CreateAuthorizedRequestAsync(HttpMethod.Get, "api/customer/profile/addresses");
        using var response = await _http.SendAsync(request, cancellationToken);

        if (!response.IsSuccessStatusCode)
            throw new InvalidOperationException(await ReadErrorMessageAsync(response));

        var result = await response.Content.ReadFromJsonAsync<List<CustomerAddressDto>>(cancellationToken: cancellationToken);
        return result ?? new List<CustomerAddressDto>();
    }

    public async Task<CustomerAddressDto> CreateAddressAsync(
        SaveCustomerAddressRequest request,
        CancellationToken cancellationToken = default)
    {
        using var httpRequest = await CreateAuthorizedRequestAsync(HttpMethod.Post, "api/customer/profile/addresses");
        httpRequest.Content = JsonContent.Create(request);

        using var response = await _http.SendAsync(httpRequest, cancellationToken);

        if (!response.IsSuccessStatusCode)
            throw new InvalidOperationException(await ReadErrorMessageAsync(response));

        return await response.Content.ReadFromJsonAsync<CustomerAddressDto>(cancellationToken: cancellationToken)
               ?? throw new InvalidOperationException("Create address returned an empty response.");
    }

    public async Task<CustomerAddressDto> UpdateAddressAsync(
        int addressId,
        SaveCustomerAddressRequest request,
        CancellationToken cancellationToken = default)
    {
        using var httpRequest = await CreateAuthorizedRequestAsync(HttpMethod.Put, $"api/customer/profile/addresses/{addressId}");
        httpRequest.Content = JsonContent.Create(request);

        using var response = await _http.SendAsync(httpRequest, cancellationToken);

        if (!response.IsSuccessStatusCode)
            throw new InvalidOperationException(await ReadErrorMessageAsync(response));

        return await response.Content.ReadFromJsonAsync<CustomerAddressDto>(cancellationToken: cancellationToken)
               ?? throw new InvalidOperationException("Update address returned an empty response.");
    }

    public async Task SetDefaultAddressAsync(int addressId, CancellationToken cancellationToken = default)
    {
        using var request = await CreateAuthorizedRequestAsync(HttpMethod.Put, $"api/customer/profile/addresses/{addressId}/default");
        using var response = await _http.SendAsync(request, cancellationToken);

        if (!response.IsSuccessStatusCode)
            throw new InvalidOperationException(await ReadErrorMessageAsync(response));
    }

    public async Task DeleteAddressAsync(int addressId, CancellationToken cancellationToken = default)
    {
        using var request = await CreateAuthorizedRequestAsync(HttpMethod.Delete, $"api/customer/profile/addresses/{addressId}");
        using var response = await _http.SendAsync(request, cancellationToken);

        if (!response.IsSuccessStatusCode)
            throw new InvalidOperationException(await ReadErrorMessageAsync(response));
    }

    public async Task<bool> TryRestoreSessionAsync(CancellationToken cancellationToken = default)
    {
        var token = await _tokenStore.GetTokenAsync();
        if (string.IsNullOrWhiteSpace(token))
        {
            _session.Clear();
            return false;
        }

        var me = await GetMeAsync(cancellationToken);
        if (me is null)
        {
            await _tokenStore.ClearTokenAsync();
            _session.Clear();
            return false;
        }

        _session.SetAuthenticated(new AuthResponse
        {
            Token = token,
            AppUserId = me.AppUserId,
            CustomerProfileId = me.CustomerProfileId,
            UserName = me.UserName,
            Email = me.Email,
            FullName = me.FullName,
            Role = "Customer"
        }, token);

        _session.SetProfile(me);
        return true;
    }

    public async Task LogoutAsync()
    {
        await _tokenStore.ClearTokenAsync();
        _session.Clear();
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
                return $"Request failed with status code {(int)response.StatusCode}.";

            var parsed = JsonSerializer.Deserialize<MessageResponse>(raw, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return !string.IsNullOrWhiteSpace(parsed?.Message)
                ? parsed.Message
                : raw;
        }
        catch
        {
            return $"Request failed with status code {(int)response.StatusCode}.";
        }
    }

    private sealed class MessageResponse
    {
        public string Message { get; set; } = string.Empty;
    }
}