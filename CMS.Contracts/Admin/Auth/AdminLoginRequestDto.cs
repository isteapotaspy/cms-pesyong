namespace CMS.Contracts.Admin.Auth;

public sealed record AdminLoginRequestDto
{
    public string EmailOrUsername { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}