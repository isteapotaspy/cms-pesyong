using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using CMS.Contracts.Admin.Package;
using CMS.Domain.Entities.Menu;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;

namespace PESYONG.Presentation.Admin.ViewModel;

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

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ImageStatusText))]
    private string imageUrl = string.Empty;

    [ObservableProperty] private decimal rating;
    [ObservableProperty] private int reviewCount;
    [ObservableProperty] private bool isAvailable = true;
    [ObservableProperty] private bool isCustomizable;

    // Temporary local image storage.
    // Later, your image service can upload this byte array and return a URL.
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasUploadedImage))]
    [NotifyPropertyChangedFor(nameof(ImageStatusText))]
    private byte[]? uploadedImageBytes;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ImageStatusText))]
    private string uploadedImageFileName = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ImageStatusText))]
    private string uploadedImageLocalPath = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasImagePreview))]
    [NotifyPropertyChangedFor(nameof(ImagePlaceholderText))]
    [NotifyPropertyChangedFor(nameof(ImageStatusText))]
    private ImageSource? imagePreviewSource;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ImageStatusText))]
    private string imageErrorMessage = string.Empty;

    public ObservableCollection<PackageSizeItemVM> Sizes { get; } = new();
    public ObservableCollection<PackageAddonItemVM> Addons { get; } = new();
    public ObservableCollection<PackageSelectionRuleItemVM> SelectionRules { get; } = new();

    public bool HasUploadedImage => UploadedImageBytes is { Length: > 0 };

    public bool HasImagePreview => ImagePreviewSource is not null;

    public string ImagePlaceholderText => HasImagePreview
        ? string.Empty
        : "No image available.";

    public string ImageStatusText
    {
        get
        {
            if (!string.IsNullOrWhiteSpace(ImageErrorMessage))
                return ImageErrorMessage;

            if (HasUploadedImage)
                return $"Selected local image: {UploadedImageFileName}";

            if (!string.IsNullOrWhiteSpace(ImageUrl))
            {
                return ImagePreviewSource is null
                    ? "Image URL is saved, but the preview cannot be loaded yet."
                    : $"Using image URL: {ImageUrl}";
            }

            return "No image selected.";
        }
    }

    partial void OnImageUrlChanged(string value)
    {
        if (HasUploadedImage)
            return;

        RefreshImagePreviewFromImageUrl();
    }

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

        ClearUploadedImageState();

        // This automatically triggers OnImageUrlChanged(),
        // which loads the preview immediately.
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

    [RelayCommand]
    private void PickImage()
    {
        var dialog = new OpenFileDialog
        {
            Title = "Select package image",
            CheckFileExists = true,
            Multiselect = false,
            Filter = "Image files (*.png;*.jpg;*.jpeg;*.bmp;*.gif)|*.png;*.jpg;*.jpeg;*.bmp;*.gif|All files (*.*)|*.*"
        };

        if (dialog.ShowDialog() != true)
            return;

        try
        {
            SetUploadedImage(dialog.FileName);
        }
        catch (Exception ex)
        {
            ClearUploadedImageState();
            ImagePreviewSource = null;
            ImageErrorMessage = $"Unable to load selected image: {ex.Message}";
        }
    }

    [RelayCommand]
    private void ClearUploadedImage()
    {
        ClearUploadedImageState();
        RefreshImagePreviewFromImageUrl();
    }

    private void SetUploadedImage(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            return;

        UploadedImageBytes = File.ReadAllBytes(filePath);
        UploadedImageLocalPath = filePath;
        UploadedImageFileName = Path.GetFileName(filePath);
        ImagePreviewSource = CreateImageSourceFromFile(filePath);
        ImageErrorMessage = string.Empty;
    }

    private void ClearUploadedImageState()
    {
        UploadedImageBytes = null;
        UploadedImageLocalPath = string.Empty;
        UploadedImageFileName = string.Empty;
        ImageErrorMessage = string.Empty;
    }

    private void RefreshImagePreviewFromImageUrl()
    {
        ImageErrorMessage = string.Empty;

        if (string.IsNullOrWhiteSpace(ImageUrl))
        {
            ImagePreviewSource = null;
            return;
        }

        ImagePreviewSource = TryCreateImageSource(ImageUrl);

        if (ImagePreviewSource is null)
            ImageErrorMessage = "Image URL is saved, but the preview cannot be loaded yet.";
    }

    private static ImageSource? TryCreateImageSource(string source)
    {
        var trimmedSource = source.Trim();

        try
        {
            if (File.Exists(trimmedSource))
                return CreateImageSourceFromFile(trimmedSource);

            if (Uri.TryCreate(trimmedSource, UriKind.Absolute, out var absoluteUri))
                return CreateImageSourceFromUri(absoluteUri);

            return null;
        }
        catch
        {
            return null;
        }
    }

    private static ImageSource CreateImageSourceFromFile(string filePath)
    {
        return CreateImageSourceFromUri(new Uri(filePath, UriKind.Absolute));
    }

    private static ImageSource CreateImageSourceFromUri(Uri uri)
    {
        var bitmap = new BitmapImage();

        bitmap.BeginInit();
        bitmap.CacheOption = BitmapCacheOption.OnLoad;
        bitmap.UriSource = uri;
        bitmap.EndInit();

        if (bitmap.CanFreeze)
            bitmap.Freeze();

        return bitmap;
    }
}