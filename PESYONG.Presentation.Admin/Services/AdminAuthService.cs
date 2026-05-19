using CMS.Contracts.Admin.Auth;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace PESYONG.Presentation.Admin.Services;

public sealed class AdminAuthService : IAdminAuthService
{
    private readonly HttpClient _httpClient;
    private readonly IAdminTokenStore _tokenStore;
    private readonly AdminSession _session;

    public AdminAuthService(
        HttpClient httpClient,
        IAdminTokenStore tokenStore,
        AdminSession session)
    {
        _httpClient = httpClient;
        _tokenStore = tokenStore;
        _session = session;
    }

    public async Task<AdminLoginResponseDto> LoginAsync(AdminLoginRequestDto request)
    {
        var response = await _httpClient.PostAsJsonAsync("api/admin/auth/login", request);
        return await ReadResponseAsync<AdminLoginResponseDto>(response);
    }

    public async Task<AdminLoginResponseDto> VerifyCodeAsync(AdminVerifyCodeRequestDto request)
    {
        var response = await _httpClient.PostAsJsonAsync("api/admin/auth/verify-code", request);
        return await ReadResponseAsync<AdminLoginResponseDto>(response);
    }

    public async Task ResendCodeAsync(AdminResendCodeRequestDto request)
    {
        var response = await _httpClient.PostAsJsonAsync("api/admin/auth/resend-code", request);

        if (!response.IsSuccessStatusCode)
            throw new InvalidOperationException(await response.Content.ReadAsStringAsync());
    }

    public async Task<AdminMeDto?> GetMeAsync()
    {
        ApplyToken();

        var response = await _httpClient.GetAsync("api/admin/auth/me");

        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<AdminMeDto>();
    }

    public async Task<bool> TryRestoreSessionAsync()
    {
        var token = await _tokenStore.GetTokenAsync();

        if (string.IsNullOrWhiteSpace(token))
            return false;

        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var me = await GetMeAsync();

        if (me is null)
        {
            await LogoutAsync();
            return false;
        }

        _session.SetSession(token, me);
        return true;
    }

    public async Task LogoutAsync()
    {
        await _tokenStore.ClearTokenAsync();

        _httpClient.DefaultRequestHeaders.Authorization = null;

        _session.Clear();
    }

    private void ApplyToken()
    {
        if (string.IsNullOrWhiteSpace(_session.Token))
            return;

        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", _session.Token);
    }

    private async Task<T> ReadResponseAsync<T>(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<T>();

            return result is null
                ? throw new InvalidOperationException("Empty API response.")
                : result;
        }

        var error = await response.Content.ReadAsStringAsync();

        throw new InvalidOperationException(
            string.IsNullOrWhiteSpace(error)
                ? "Request failed."
                : error);
    }
}