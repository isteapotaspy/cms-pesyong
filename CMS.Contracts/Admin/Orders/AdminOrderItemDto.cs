using System;
using System.Collections.Generic;
using System.Text;

namespace CMS.Contracts.Admin.Orders;

public class AdminOrderItemDto
{
    public int Id { get; set; }

    public string PackageTitle { get; set; } = string.Empty;
    public string SizeLabel { get; set; } = string.Empty;

    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal LineTotal { get; set; }

    public List<string> SelectedMeals { get; set; } = new();
    public List<string> SelectedAddons { get; set; } = new();
}
