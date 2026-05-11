using System;
using System.Collections.Generic;
using System.Text;

namespace CMS.Contracts.Customer.Meals;

public class GetCustomerMealsResponse
{
    public List<CustomerMealDto> Meals { get; set; } = new();
}
