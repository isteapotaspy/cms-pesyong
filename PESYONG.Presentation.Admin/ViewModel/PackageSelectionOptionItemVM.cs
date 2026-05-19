using System;
using System.Collections.Generic;
using System.Text;
using CMS.Contracts.Admin.Package;
using CommunityToolkit.Mvvm.ComponentModel;

namespace PESYONG.Presentation.Admin.ViewModel;

public partial class PackageSelectionOptionItemVM : ObservableObject
{
    [ObservableProperty]
    private int id;

    [ObservableProperty]
    private int mealId;

    [ObservableProperty]
    private string mealName = string.Empty;

    [ObservableProperty]
    private decimal additionalPrice;

    [ObservableProperty]
    private bool isDefault;

    public static PackageSelectionOptionItemVM FromDto(PackageSelectionOptionDto dto)
    {
        return new PackageSelectionOptionItemVM
        {
            Id = dto.Id,
            MealId = dto.MealId,
            MealName = dto.MealName,
            AdditionalPrice = dto.AdditionalPrice,
            IsDefault = dto.IsDefault
        };
    }

    public PackageSelectionOptionRequest ToRequest()
    {
        return new PackageSelectionOptionRequest
        {
            MealId = MealId,
            AdditionalPrice = AdditionalPrice,
            IsDefault = IsDefault
        };
    }
}