using System;
using System.Collections.Generic;
using System.Text;

namespace CMS.Contracts.Customer.Menu;
public class MenuCategoryDto
{
    public int Id { get; set; } 
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
}
