using System;
using System.Collections.Generic;
using System.Text;

namespace CMS.Contracts.Admin.Promos;

public sealed record class PromoDto
{
    public int Id { get; init; }

    public DateTime? DateCreated { get; init; }
    public DateTime? DateUpdated { get; init; }

    public string Code { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;

    public decimal DiscountPercentageValue { get; init; }
    public decimal? MinimumOrderAmount { get; init; }

    public int? UsageLimit { get; init; }
    public int UsedCount { get; init; }

    public DateTime ValidFromUtc { get; init; }
    public DateTime ValidUntilUtc { get; init; }

    public bool IsActive { get; init; }
}
