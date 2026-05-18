using System;
using System.Collections.Generic;
using System.Text;

namespace CMS.Contracts.Admin.Payment;

public sealed record class UpdatePaymentRequest
{
    public int OrderId { get; init; }

    public string ExternalReference { get; init; } = string.Empty;

    public string PaymentMethod { get; init; } = string.Empty;

    public string PaymentStatus { get; init; } = string.Empty;

    public DateTime TimestampUtc { get; init; }

    public decimal Amount { get; init; }

    public string Description { get; init; } = string.Empty;
}
