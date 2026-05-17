using System;
using System.Collections.Generic;
using System.Text;

namespace CMS.Contracts.Admin.Payment;

public sealed record class PaymentDto
{
    public int Id { get; init; }

    public int OrderId { get; init; }

    public string ExternalReference { get; init; } = string.Empty;

    public string PaymentMethod { get; init; } = string.Empty;

    public string PaymentStatus { get; init; } = string.Empty;

    public DateTime TimestampUtc { get; init; }

    public decimal Amount { get; init; }

    public string Description { get; init; } = string.Empty;

    public DateTime? DateCreated { get; init; }

    public DateTime? DateUpdated { get; init; }

    public string OrderDisplay { get; init; } = string.Empty;
}
