namespace CMS.Contracts.Admin.Auth;

public sealed record AdminResendCodeRequestDto
{
    public string EmailOrUsername { get; init; } = string.Empty;
}