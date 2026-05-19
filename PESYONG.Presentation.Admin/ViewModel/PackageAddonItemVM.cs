using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using CMS.Contracts.Admin.Package;
using CommunityToolkit.Mvvm.ComponentModel;

namespace PESYONG.Presentation.Admin.ViewModel;

public partial class PackageAddonItemVM : ObservableObject
{
    [ObservableProperty]
    private int id;

    [ObservableProperty]
    private string name = string.Empty;

    [ObservableProperty]
    private string description = string.Empty;

    [ObservableProperty]
    private decimal price;

    [ObservableProperty]
    private bool isAvailable = true;

    public static PackageAddonItemVM FromDto(PackageAddonDto dto)
    {
        return new PackageAddonItemVM
        {
            Id = dto.Id,
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price,
            IsAvailable = dto.IsAvailable
        };
    }

    public PackageAddonRequest ToRequest()
    {
        return new PackageAddonRequest
        {
            Name = Name,
            Description = Description,
            Price = Price,
            IsAvailable = IsAvailable
        };
    }
}
