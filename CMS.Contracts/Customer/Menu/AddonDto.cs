using System;
using System.Collections.Generic;
using System.Text;

namespace CMS.Contracts.Customer.Menu;

public class AddonDto
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Price { get; set; }
}
