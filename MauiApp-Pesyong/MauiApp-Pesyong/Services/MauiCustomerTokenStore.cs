using MauiApp_Pesyong.Shared.Customer_Main.Customer_Services;

namespace MauiApp_Pesyong.Services;

public class MauiCustomerTokenStore : ICustomerTokenStore
{
    private const string TokenKey = "customer_auth_token";

    public async Task<string?> GetTokenAsync()
    {
        try
        {
            return await SecureStorage.Default.GetAsync(TokenKey);
        }
        catch
        {
            return null;
        }
    }

    public async Task SetTokenAsync(string token)
    {
        await SecureStorage.Default.SetAsync(TokenKey, token);
    }

    public Task ClearTokenAsync()
    {
        SecureStorage.Default.Remove(TokenKey);
        return Task.CompletedTask;
    }
}