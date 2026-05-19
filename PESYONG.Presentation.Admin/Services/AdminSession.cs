using CMS.Contracts.Admin.Auth;

namespace PESYONG.Presentation.Admin.Services;

public sealed class AdminSession
{
    public bool IsAuthenticated => !string.IsNullOrWhiteSpace(Token);

    public string Token { get; private set; } = string.Empty;

    public AdminMeDto? Admin { get; private set; }

    public void SetSession(string token, AdminMeDto? admin)
    {
        Token = token;
        Admin = admin;
    }

    public void Clear()
    {
        Token = string.Empty;
        Admin = null;
    }
}