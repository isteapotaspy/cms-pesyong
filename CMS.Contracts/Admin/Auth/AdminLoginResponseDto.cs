namespace CMS.Contracts.Admin.Auth;

public sealed record AdminLoginResponseDto
{
    public bool Succeeded { get; init; }
    public bool RequiresVerification { get; init; }

    public string Message { get; init; } = string.Empty;
    public string Token { get; init; } = string.Empty;

    public AdminMeDto? Admin { get; init; }
}