
namespace MauiApp_Pesyong.Shared.Customer_Main.Customer_Services;

public interface ICustomerTokenStore
{
    Task<string?> GetTokenAsync();
    Task SetTokenAsync(string token);
    Task ClearTokenAsync();
}