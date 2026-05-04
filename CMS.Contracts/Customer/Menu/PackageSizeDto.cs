using System;
using System.Collections.Generic;
using System.Text;

namespace CMS.Contracts.Customer.Menu;
public class PackageSizeDto
{
    public string Id { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string Subtitle { get; set; } = string.Empty;
    public decimal Price { get; set; }
}
