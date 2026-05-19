using Microsoft.JSInterop;
using MauiApp_Pesyong.Shared.Customer_Main.Customer_Services;

namespace MauiApp_Pesyong.Web.Services;

public class WebCustomerTokenStore : ICustomerTokenStore
{
    private const string TokenKey = "customer_auth_token";
    private readonly IJSRuntime _jsRuntime;

    public WebCustomerTokenStore(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task<string?> GetTokenAsync()
    {
        try
        {
            return await _jsRuntime.InvokeAsync<string?>("localStorage.getItem", TokenKey);
        }
        catch
        {
            return null;
        }
    }

    public async Task SetTokenAsync(string token)
    {
        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", TokenKey, token);
    }

    public async Task ClearTokenAsync()
    {
        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", TokenKey);
    }
}