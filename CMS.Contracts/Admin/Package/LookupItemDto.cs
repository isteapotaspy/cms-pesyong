using System;
using System.Collections.Generic;
using System.Text;

namespace CMS.Contracts.Admin.Package;

public sealed record LookupItemDto
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
}