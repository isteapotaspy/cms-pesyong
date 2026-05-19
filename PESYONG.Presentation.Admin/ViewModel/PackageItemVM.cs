using System.Collections.ObjectModel;
using System.ComponentModel.Design;
using System.Windows.Media;
using System.Xml.Linq;
using CMS.Contracts.Admin.Package;
using CMS.Domain.Entities.Menu;
using CommunityToolkit.Mvvm.ComponentModel;

namespace PESYONG.Presentation.Admin.ViewModel;

public partial class PackageItemVM : ObservableObject
{
    [ObservableProperty]
    private int id;

    [ObservableProperty]
    private int menuCategoryId;

    [ObservableProperty]
    private string menuCategoryName = string.Empty;

    [ObservableProperty]
    private string title = string.Empty;

    [ObservableProperty]
    private string description = string.Empty;

    [ObservableProperty]
    private string cardSummary = string.Empty;

    [ObservableProperty]
    private string badge = string.Empty;

    [ObservableProperty]
    private string notice = string.Empty;

    [ObservableProperty]
    private string servesLabel = string.Empty;

    [ObservableProperty]
    private string inclusionText = string.Empty;

    [ObservableProperty]
    private string imageUrl = string.Empty;

    [ObservableProperty]
    private decimal rating;

    [ObservableProperty]
    private int reviewCount;

    [ObservableProperty]
    private bool isAvailable = true;

    [ObservableProperty]
    private bool isCustomizable = true;

    public ObservableCollection<PackageSizeItemVM> Sizes { get; } = new();
    public ObservableCollection<PackageAddonItemVM> Addons { get; } = new();

    public string ListTitle => Id == 0 ? "New Package" : Title;

    public string ListSummary =>
        Sizes.Count == 0
            ? "No sizes"
            : $"{Sizes.Count} size(s), {Addons.Count} addon(s)";

    public static PackageItemVM FromDto(PackageDto dto)
    {
        var vm = new PackageItemVM
        {
            Id = dto.Id,
            MenuCategoryId = dto.MenuCategoryId,
            MenuCategoryName = dto.MenuCategoryName,

            Title = dto.Title,
            Description = dto.Description,
            CardSummary = dto.CardSummary,
            Badge = dto.Badge,
            Notice = dto.Notice,
            ServesLabel = dto.ServesLabel,
            InclusionText = dto.InclusionText,
            ImageUrl = dto.ImageUrl,

            Rating = dto.Rating,
            ReviewCount = dto.ReviewCount,
            IsAvailable = dto.IsAvailable,
            IsCustomizable = dto.IsCustomizable
        };

        foreach (var size in dto.Sizes)
        {
            vm.Sizes.Add(PackageSizeItemVM.FromDto(size));
        }

        foreach (var addon in dto.Addons)
        {
            vm.Addons.Add(PackageAddonItemVM.FromDto(addon));
        }

        return vm;
    }

    public void CopyFrom(PackageDto dto)
    {
        Id = dto.Id;
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

        OnPropertyChanged(nameof(ListTitle));
        OnPropertyChanged(nameof(ListSummary));
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
            Addons = Addons.Select(addon => addon.ToRequest()).ToList()
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
            Addons = Addons.Select(addon => addon.ToRequest()).ToList()
        };
    }

    partial void OnIdChanged(int value) => OnPropertyChanged(nameof(ListTitle));
    partial void OnTitleChanged(string value) => OnPropertyChanged(nameof(ListTitle));
}







