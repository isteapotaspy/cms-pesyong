namespace CMS.Contracts.Admin.Auth;

public sealed record AdminMeDto
{
    public int Id { get; init; }

    public string UserName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;

    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;

    public string FullName => $"{FirstName} {LastName}".Trim();

    public string Role { get; init; } = string.Empty;
}