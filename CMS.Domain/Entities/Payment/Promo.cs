using CMS.Domain.Common;

namespace CMS.Domain.Entities.Payment;

public class Promo : BaseEntity
{
    public string Code { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public decimal DiscountPercentageValue { get; set; }
    public decimal? MinimumOrderAmount { get; set; }

    public int? UsageLimit { get; set; }
    public int UsedCount { get; set; }

    public DateTime ValidFromUtc { get; set; }
    public DateTime ValidUntilUtc { get; set; }

    public bool IsActive =>
        DateTime.UtcNow >= ValidFromUtc &&
        DateTime.UtcNow <= ValidUntilUtc &&
        (!UsageLimit.HasValue || UsedCount < UsageLimit.Value);
}