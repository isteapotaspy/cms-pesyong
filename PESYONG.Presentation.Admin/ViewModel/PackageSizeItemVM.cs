
using System.Collections.ObjectModel;
using CMS.Contracts.Admin.Package;
using CommunityToolkit.Mvvm.ComponentModel;

namespace PESYONG.Presentation.Admin.ViewModel;

public partial class PackageSizeItemVM : ObservableObject
{
    [ObservableProperty]
    private int id;

    [ObservableProperty]
    private string label = string.Empty;

    [ObservableProperty]
    private string subtitle = string.Empty;

    [ObservableProperty]
    private int paxCount = 1;

    [ObservableProperty]
    private decimal price;

    public ObservableCollection<PackageSelectionRuleItemVM> SelectionRules { get; } = new();

    public static PackageSizeItemVM FromDto(PackageSizeDto dto)
    {
        var vm = new PackageSizeItemVM
        {
            Id = dto.Id,
            Label = dto.Label,
            Subtitle = dto.Subtitle,
            PaxCount = dto.PaxCount,
            Price = dto.Price
        };

        foreach (var rule in dto.SelectionRules)
        {
            vm.SelectionRules.Add(PackageSelectionRuleItemVM.FromDto(rule));
        }

        return vm;
    }

    public PackageSizeRequest ToRequest()
    {
        return new PackageSizeRequest
        {
            Label = Label,
            Subtitle = Subtitle,
            PaxCount = PaxCount,
            Price = Price,
            SelectionRules = SelectionRules
                .Select(rule => rule.ToRequest())
                .ToList()
        };
    }
}