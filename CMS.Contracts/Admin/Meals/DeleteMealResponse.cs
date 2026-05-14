using System;
using System.Collections.Generic;
using System.Text;

namespace CMS.Contracts.Admin.Meals;

public class DeleteMealResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool IsAvailable { get; set; }
    public string Message { get; set; } = string.Empty;
}
