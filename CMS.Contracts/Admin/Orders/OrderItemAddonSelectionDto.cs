using System;
using System.Collections.Generic;
using System.Text;

namespace CMS.Contracts.Admin.Orders;

public sealed record OrderItemAddonSelectionDto
{
    public int Id { get; init; }

    public int OrderItemId { get; init; }
    public int PackageAddonId { get; init; }

    public string AddonNameSnapshot { get; init; } = string.Empty;
    public decimal AdditionalPrice { get; init; }
}
