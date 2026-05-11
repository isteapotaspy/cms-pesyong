using System;
using System.Collections.Generic;
using System.Text;

namespace CMS.Contracts.Customer.Orders;
public class OrderItemMealSelectionRequestDto
{
    public int PackageSelectionRuleId { get; set; }
    public int MealId { get; set; }
}
