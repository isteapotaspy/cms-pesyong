using System;
using System.Collections.Generic;
using System.Text;

namespace CMS.Contracts.Admin.Package;

public sealed record PackageSizeRequest
{
    public string Label { get; init; } = string.Empty;
    public string Subtitle { get; init; } = string.Empty;
    public int PaxCount { get; init; }
    public decimal Price { get; init; }
}