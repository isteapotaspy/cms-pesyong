using CMS.Contracts.Customer.Auth;

namespace MauiApp_Pesyong.Shared.Customer_Main.Customer_Services;

public class CustomerSession
{
    public bool IsAuthenticated { get; private set; }
    public string? Token { get; private set; }
    public AuthResponse? Auth { get; private set; }
    public CustomerMeResponse? Profile { get; private set; }

    public event Action? OnChange;

    public void SetAuthenticated(AuthResponse auth, string token)
    {
        Auth = auth;
        Token = token;
        IsAuthenticated = true;
        NotifyStateChanged();
    }

    public void SetProfile(CustomerMeResponse profile)
    {
        Profile = profile;
        NotifyStateChanged();
    }

    public void Clear()
    {
        Auth = null;
        Token = null;
        Profile = null;
        IsAuthenticated = false;
        NotifyStateChanged();
    }

    private void NotifyStateChanged() => OnChange?.Invoke();
}