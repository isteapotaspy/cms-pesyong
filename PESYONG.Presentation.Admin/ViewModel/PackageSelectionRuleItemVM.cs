using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using CMS.Contracts.Admin.Package;
using CommunityToolkit.Mvvm.ComponentModel;

namespace PESYONG.Presentation.Admin.ViewModel;

public partial class PackageSelectionRuleItemVM : ObservableObject
{
    [ObservableProperty]
    private int id;

    [ObservableProperty]
    private string title = string.Empty;

    [ObservableProperty]
    private string description = string.Empty;

    [ObservableProperty]
    private string selectionType = "ChooseOne";

    [ObservableProperty]
    private string allowedMealType = "Viand";

    [ObservableProperty]
    private int minSelections = 1;

    [ObservableProperty]
    private int maxSelections = 1;

    [ObservableProperty]
    private bool isRequired = true;

    [ObservableProperty]
    private int displayOrder;

    public ObservableCollection<PackageSelectionOptionItemVM> Options { get; } = new();

    public static PackageSelectionRuleItemVM FromDto(PackageSelectionRuleDto dto)
    {
        var vm = new PackageSelectionRuleItemVM
        {
            Id = dto.Id,
            Title = dto.Title,
            Description = dto.Description,
            SelectionType = dto.SelectionType,
            AllowedMealType = dto.AllowedMealType,
            MinSelections = dto.MinSelections,
            MaxSelections = dto.MaxSelections,
            IsRequired = dto.IsRequired,
            DisplayOrder = dto.DisplayOrder
        };

        foreach (var option in dto.Options)
        {
            vm.Options.Add(PackageSelectionOptionItemVM.FromDto(option));
        }

        return vm;
    }

    public PackageSelectionRuleRequest ToRequest()
    {
        return new PackageSelectionRuleRequest
        {
            Title = Title,
            Description = Description,
            SelectionType = SelectionType,
            AllowedMealType = AllowedMealType,
            MinSelections = MinSelections,
            MaxSelections = MaxSelections,
            IsRequired = IsRequired,
            DisplayOrder = DisplayOrder,
            Options = Options.Select(option => option.ToRequest()).ToList()
        };
    }
}