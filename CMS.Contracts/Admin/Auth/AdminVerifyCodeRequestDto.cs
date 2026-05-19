namespace CMS.Contracts.Admin.Auth;

public sealed record AdminVerifyCodeRequestDto
{
    public string EmailOrUsername { get; init; } = string.Empty;
    public string Code { get; init; } = string.Empty;
}