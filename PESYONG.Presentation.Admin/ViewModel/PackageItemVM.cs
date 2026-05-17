using System.Collections.ObjectModel;
using System.Windows.Media;
using CMS.Contracts.Admin.Package;
using CommunityToolkit.Mvvm.ComponentModel;

namespace PESYONG.Presentation.Admin.ViewModels.Packages;

public partial class PackageItemVM : ObservableObject
{
    [ObservableProperty] private int id;
    [ObservableProperty] private DateTime? createdAt;
    [ObservableProperty] private DateTime? updatedAt;

    [ObservableProperty] private int menuCategoryId;
    [ObservableProperty] private string menuCategoryName = string.Empty;

    [ObservableProperty] private string title = string.Empty;
    [ObservableProperty] private string description = string.Empty;
    [ObservableProperty] private string cardSummary = string.Empty;
    [ObservableProperty] private string badge = string.Empty;
    [ObservableProperty] private string notice = string.Empty;
    [ObservableProperty] private string servesLabel = string.Empty;
    [ObservableProperty] private string inclusionText = string.Empty;
    [ObservableProperty] private string imageUrl = string.Empty;

    [ObservableProperty] private decimal rating;
    [ObservableProperty] private int reviewCount;
    [ObservableProperty] private bool isAvailable = true;
    [ObservableProperty] private bool isCustomizable;

    public ObservableCollection<PackageSizeItemVM> Sizes { get; } = new();
    public ObservableCollection<PackageAddonItemVM> Addons { get; } = new();
    public ObservableCollection<PackageSelectionRuleItemVM> SelectionRules { get; } = new();

    public static PackageItemVM FromDto(PackageDto dto)
    {
        var vm = new PackageItemVM();
        vm.CopyFrom(dto);
        return vm;
    }

    public void CopyFrom(PackageDto dto)
    {
        Id = dto.Id;
        CreatedAt = dto.CreatedAt;
        UpdatedAt = dto.UpdatedAt;

        MenuCategoryId = dto.MenuCategoryId;
        MenuCategoryName = dto.MenuCategoryName;

        Title = dto.Title;
        Description = dto.Description;
        CardSummary = dto.CardSummary;
        Badge = dto.Badge;
        Notice = dto.Notice;
        ServesLabel = dto.ServesLabel;
        InclusionText = dto.InclusionText;
        ImageUrl = dto.ImageUrl;

        Rating = dto.Rating;
        ReviewCount = dto.ReviewCount;
        IsAvailable = dto.IsAvailable;
        IsCustomizable = dto.IsCustomizable;

        Sizes.Clear();
        foreach (var size in dto.Sizes)
        {
            Sizes.Add(PackageSizeItemVM.FromDto(size));
        }

        Addons.Clear();
        foreach (var addon in dto.Addons)
        {
            Addons.Add(PackageAddonItemVM.FromDto(addon));
        }

        SelectionRules.Clear();
        foreach (var rule in dto.SelectionRules)
        {
            SelectionRules.Add(PackageSelectionRuleItemVM.FromDto(rule));
        }
    }

    public CreatePackageRequest ToCreateRequest()
    {
        return new CreatePackageRequest
        {
            MenuCategoryId = MenuCategoryId,
            Title = Title,
            Description = Description,
            CardSummary = CardSummary,
            Badge = Badge,
            Notice = Notice,
            ServesLabel = ServesLabel,
            InclusionText = InclusionText,
            ImageUrl = ImageUrl,
            Rating = Rating,
            ReviewCount = ReviewCount,
            IsAvailable = IsAvailable,
            IsCustomizable = IsCustomizable,
            Sizes = Sizes.Select(size => size.ToRequest()).ToList(),
            Addons = Addons.Select(addon => addon.ToRequest()).ToList(),
            SelectionRules = SelectionRules.Select(rule => rule.ToRequest()).ToList()
        };
    }

    public UpdatePackageRequest ToUpdateRequest()
    {
        return new UpdatePackageRequest
        {
            Id = Id,
            MenuCategoryId = MenuCategoryId,
            Title = Title,
            Description = Description,
            CardSummary = CardSummary,
            Badge = Badge,
            Notice = Notice,
            ServesLabel = ServesLabel,
            InclusionText = InclusionText,
            ImageUrl = ImageUrl,
            Rating = Rating,
            ReviewCount = ReviewCount,
            IsAvailable = IsAvailable,
            IsCustomizable = IsCustomizable,
            Sizes = Sizes.Select(size => size.ToRequest()).ToList(),
            Addons = Addons.Select(addon => addon.ToRequest()).ToList(),
            SelectionRules = SelectionRules.Select(rule => rule.ToRequest()).ToList()
        };
    }
}