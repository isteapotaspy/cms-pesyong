using System.Collections.ObjectModel;
using System.ComponentModel.Design;
using System.Xml.Linq;
using CMS.Contracts.Admin.Package;
using CMS.Contracts.Customer.Menu;
using CommunityToolkit.Mvvm.ComponentModel;

namespace PESYONG.Presentation.Admin.ViewModel;

public partial class PackageSizeItemVM : ObservableObject
{
    [ObservableProperty] private int id;
    [ObservableProperty] private string label = string.Empty;
    [ObservableProperty] private string subtitle = string.Empty;
    [ObservableProperty] private int paxCount;
    [ObservableProperty] private decimal price;

    public static PackageSizeItemVM FromDto(PackageSizeDto dto)
    {
        return new PackageSizeItemVM
        {
            Id = dto.Id,
            Label = dto.Label,
            Subtitle = dto.Subtitle,
            PaxCount = dto.PaxCount,
            Price = dto.Price
        };
    }

    public PackageSizeRequest ToRequest()
    {
        return new PackageSizeRequest
        {
            Label = Label,
            Subtitle = Subtitle,
            PaxCount = PaxCount,
            Price = Price
        };
    }
}

public partial class PackageAddonItemVM : ObservableObject
{
    [ObservableProperty] private int id;
    [ObservableProperty] private string name = string.Empty;
    [ObservableProperty] private string description = string.Empty;
    [ObservableProperty] private decimal price;
    [ObservableProperty] private bool isAvailable = true;

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

public partial class PackageSelectionOptionItemVM : ObservableObject
{
    [ObservableProperty] private int id;
    [ObservableProperty] private int mealId;
    [ObservableProperty] private string mealName = string.Empty;
    [ObservableProperty] private decimal additionalPrice;
    [ObservableProperty] private bool isDefault;

    public static PackageSelectionOptionItemVM FromDto(PackageSelectionOptionDto dto)
    {
        return new PackageSelectionOptionItemVM
        {
            Id = dto.Id,
            MealId = dto.MealId,
            MealName = dto.MealName ?? string.Empty,
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

public partial class PackageSelectionRuleItemVM : ObservableObject
{
    [ObservableProperty] private int id;
    [ObservableProperty] private string title = string.Empty;
    [ObservableProperty] private string description = string.Empty;
    [ObservableProperty] private string selectionType = string.Empty;
    [ObservableProperty] private string allowedMealType = string.Empty;
    [ObservableProperty] private int minSelections;
    [ObservableProperty] private int maxSelections = 1;
    [ObservableProperty] private bool isRequired = true;
    [ObservableProperty] private int displayOrder;

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