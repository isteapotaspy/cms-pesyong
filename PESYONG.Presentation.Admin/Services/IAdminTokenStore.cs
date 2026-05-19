namespace PESYONG.Presentation.Admin.Services;

public interface IAdminTokenStore
{
    Task SaveTokenAsync(string token);
    Task<string?> GetTokenAsync();
    Task ClearTokenAsync();
}