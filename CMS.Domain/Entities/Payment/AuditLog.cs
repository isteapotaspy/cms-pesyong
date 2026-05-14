using CMS.Domain.Common;
using CMS.Domain.Entities.User;
using CMS.Domain.Enums;

namespace CMS.Domain.Entities.Payment;

public class AuditLog : BaseEntity
{
    public int? AppUserId { get; set; }

    public AuditActionType ActionType { get; set; }
    public string EntityType { get; set; } = string.Empty;
    public int EntityId { get; set; }

    public DateTime TimestampUtc { get; set; } = DateTime.UtcNow;

    public string? OldValuesJson { get; set; }
    public string? NewValuesJson { get; set; }
    public string? ChangesJson { get; set; }

    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public string? Source { get; set; }

    public bool Success { get; set; } = true;
    public string? ErrorMessage { get; set; }

    public AppUser? AppUser { get; set; }
}