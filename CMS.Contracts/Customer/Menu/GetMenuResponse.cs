using System;
using System.Collections.Generic;
using System.Text;

namespace CMS.Contracts.Customer.Menu;
public class GetMenuResponse
{
    public List<MenuCategoryDto> Categories { get; set; } = new();
    public List<MenuPackageDto> Packages { get; set; } = new();
}