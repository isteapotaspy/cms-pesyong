using CMS.Domain.Common;
using CMS.Domain.Enums;

namespace CMS.Domain.Entities.User;

public class AppUser : BaseEntity
{
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public bool IsActive { get; set; } = true;

    public CustomerProfile? CustomerProfile { get; set; }

    public string FullName => $"{FirstName} {LastName}".Trim();
}