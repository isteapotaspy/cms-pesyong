using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using CMS.Contracts.Admin.Package;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using PESYONG.Presentation.Admin.Interfaces;

namespace PESYONG.Presentation.Admin.ViewModel;

public partial class PackagesPageVM : ObservableObject
{
    private readonly IPackageApiService _packageApiService;
    private readonly IImageApiService _imageApiService;

    private bool _isLoadingOrSaving;

    public PackagesPageVM(
        IPackageApiService packageApiService,
        IImageApiService imageApiService)
    {
        _packageApiService = packageApiService;
        _imageApiService = imageApiService;
    }

    public ObservableCollection<PackageItemVM> Packages { get; } = new();
    public ObservableCollection<LookupItemDto> MenuCategories { get; } = new();
    public ObservableCollection<LookupItemDto> Meals { get; } = new();
    public ObservableCollection<string> SelectionTypeOptions { get; } = new();
    public ObservableCollection<string> MealTypeOptions { get; } = new();

    [ObservableProperty]
    private PackageItemVM? selectedPackage;

    [ObservableProperty]
    private PackageSizeItemVM? selectedSize;

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private bool isEditing;

    [ObservableProperty]
    private bool isModified;

    [ObservableProperty]
    private string errorMessage = string.Empty;

    [ObservableProperty]
    private string selectedLocalImagePath = string.Empty;

    [ObservableProperty]
    private ImageSource? selectedImagePreviewSource;

    [ObservableProperty]
    private string imageStatusText = "No image selected.";

    public bool IsNotEditing => !IsEditing;

    public string EditSaveButtonText => IsEditing ? "Save" : "Edit";

    public string ImagePlaceholderText =>
        SelectedImagePreviewSource is null
            ? "No image selected."
            : string.Empty;

    partial void OnSelectedPackageChanged(PackageItemVM? oldValue, PackageItemVM? newValue)
    {
        if (oldValue is not null)
        {
            oldValue.PropertyChanged -= SelectedPackage_PropertyChanged;
        }

        if (newValue is not null)
        {
            newValue.PropertyChanged += SelectedPackage_PropertyChanged;
        }

        SelectedSize = newValue?.Sizes.FirstOrDefault();

        if (!_isLoadingOrSaving)
        {
            IsEditing = false;
            IsModified = false;
            RefreshImagePreviewFromSelectedPackage();
        }

        UpdateCommandStates();
    }

    partial void OnSelectedSizeChanged(PackageSizeItemVM? value)
    {
        UpdateCommandStates();
    }

    partial void OnIsBusyChanged(bool value)
    {
        UpdateCommandStates();
    }

    partial void OnIsEditingChanged(bool value)
    {
        OnPropertyChanged(nameof(IsNotEditing));
        OnPropertyChanged(nameof(EditSaveButtonText));

        UpdateCommandStates();
    }

    partial void OnIsModifiedChanged(bool value)
    {
        UpdateCommandStates();
    }

    partial void OnSelectedImagePreviewSourceChanged(ImageSource? value)
    {
        OnPropertyChanged(nameof(ImagePlaceholderText));
    }

    private void SelectedPackage_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (_isLoadingOrSaving)
        {
            return;
        }

        if (IsEditing)
        {
            IsModified = true;
            UpdateCommandStates();
        }
    }

    private bool CanLoad()
    {
        return !IsBusy;
    }

    private bool CanNewPackage()
    {
        return !IsBusy;
    }

    private bool CanEditOrSave()
    {
        if (IsBusy || SelectedPackage is null)
        {
            return false;
        }

        if (!IsEditing)
        {
            return SelectedPackage.Id > 0;
        }

        return true;
    }

    private bool CanDelete()
    {
        return !IsBusy &&
               !IsEditing &&
               SelectedPackage is not null &&
               SelectedPackage.Id > 0;
    }

    private bool CanUseImageCommands()
    {
        return !IsBusy &&
               IsEditing &&
               SelectedPackage is not null;
    }

    [RelayCommand(CanExecute = nameof(CanLoad))]
    private async Task LoadAsync()
    {
        await RunSafeAsync(async () =>
        {
            _isLoadingOrSaving = true;
            IsEditing = false;

            await LoadLookupsAsync();

            var packages = await _packageApiService.GetAllAsync();

            Packages.Clear();

            foreach (var package in packages)
            {
                Packages.Add(PackageItemVM.FromDto(package));
            }

            SelectedPackage = Packages.FirstOrDefault();
            SelectedSize = SelectedPackage?.Sizes.FirstOrDefault();

            IsEditing = false;
            IsModified = false;

            _isLoadingOrSaving = false;

            RefreshImagePreviewFromSelectedPackage();
        });
    }

    private async Task LoadLookupsAsync()
    {
        var lookups = await _packageApiService.GetLookupsAsync();

        MenuCategories.Clear();
        foreach (var category in lookups.MenuCategories)
        {
            MenuCategories.Add(category);
        }

        Meals.Clear();
        foreach (var meal in lookups.Meals)
        {
            Meals.Add(meal);
        }

        SelectionTypeOptions.Clear();
        foreach (var selectionType in lookups.PackageSelectionTypes)
        {
            SelectionTypeOptions.Add(selectionType);
        }

        MealTypeOptions.Clear();
        foreach (var mealType in lookups.MealTypes)
        {
            MealTypeOptions.Add(mealType);
        }
    }

    [RelayCommand(CanExecute = nameof(CanNewPackage))]
    private void NewPackage()
    {
        _isLoadingOrSaving = true;

        var defaultSize = new PackageSizeItemVM
        {
            Label = "New Size",
            Subtitle = string.Empty,
            PaxCount = 1,
            Price = 0
        };

        var package = new PackageItemVM
        {
            MenuCategoryId = MenuCategories.FirstOrDefault()?.Id ?? 0,
            MenuCategoryName = MenuCategories.FirstOrDefault()?.Name ?? string.Empty,
            Title = string.Empty,
            Description = string.Empty,
            CardSummary = string.Empty,
            Badge = string.Empty,
            Notice = string.Empty,
            ServesLabel = string.Empty,
            InclusionText = string.Empty,
            ImageUrl = string.Empty,
            IsAvailable = true,
            IsCustomizable = false,
            Rating = 0,
            ReviewCount = 0
        };

        package.Sizes.Add(defaultSize);

        SelectedPackage = package;
        SelectedSize = defaultSize;

        _isLoadingOrSaving = false;

        SelectedLocalImagePath = string.Empty;
        SelectedImagePreviewSource = null;
        ImageStatusText = "No image selected.";

        IsEditing = true;
        IsModified = true;
        ErrorMessage = string.Empty;

        UpdateCommandStates();
    }

    [RelayCommand(CanExecute = nameof(CanEditOrSave))]
    private async Task EditOrSaveAsync()
    {
        if (SelectedPackage is null)
        {
            return;
        }

        if (!IsEditing)
        {
            IsEditing = true;
            IsModified = false;
            ErrorMessage = string.Empty;

            UpdateCommandStates();
            return;
        }

        await RunSafeAsync(async () =>
        {
            _isLoadingOrSaving = true;

            PackageDto savedPackage;

            if (SelectedPackage.Id == 0)
            {
                savedPackage = await _packageApiService.CreateAsync(
                    SelectedPackage.ToCreateRequest());

                var newVm = PackageItemVM.FromDto(savedPackage);

                Packages.Insert(0, newVm);

                SelectedPackage = newVm;
                SelectedSize = newVm.Sizes.FirstOrDefault();
            }
            else
            {
                savedPackage = await _packageApiService.UpdateAsync(
                    SelectedPackage.Id,
                    SelectedPackage.ToUpdateRequest());

                SelectedPackage.CopyFrom(savedPackage);
                SelectedSize = SelectedPackage.Sizes.FirstOrDefault();
            }

            IsEditing = false;
            IsModified = false;

            _isLoadingOrSaving = false;

            RefreshImagePreviewFromSelectedPackage();
        });
    }

    [RelayCommand(CanExecute = nameof(CanDelete))]
    private async Task DeleteAsync()
    {
        if (SelectedPackage is null || SelectedPackage.Id == 0)
        {
            return;
        }

        await RunSafeAsync(async () =>
        {
            _isLoadingOrSaving = true;

            var packageToDelete = SelectedPackage;

            await _packageApiService.DeleteAsync(packageToDelete.Id);

            Packages.Remove(packageToDelete);

            SelectedPackage = Packages.FirstOrDefault();
            SelectedSize = SelectedPackage?.Sizes.FirstOrDefault();

            IsEditing = false;
            IsModified = false;

            _isLoadingOrSaving = false;

            RefreshImagePreviewFromSelectedPackage();
        });
    }

    [RelayCommand(CanExecute = nameof(CanUseImageCommands))]
    private async Task UploadPackageImageFromDeviceAsync()
    {
        if (SelectedPackage is null)
        {
            return;
        }

        var dialog = new OpenFileDialog
        {
            Title = "Choose package image",
            Filter = "Image files (*.jpg;*.jpeg;*.png;*.webp)|*.jpg;*.jpeg;*.png;*.webp",
            Multiselect = false
        };

        if (dialog.ShowDialog() != true)
        {
            return;
        }

        await RunSafeAsync(async () =>
        {
            SelectedLocalImagePath = dialog.FileName;
            SelectedImagePreviewSource = CreateBitmapImage(dialog.FileName);
            ImageStatusText = "Uploading image...";

            var uploadedImage = await _imageApiService.UploadImageAsync(dialog.FileName);

            SelectedPackage.ImageUrl = uploadedImage.ImageUrl;

            IsModified = true;
            ImageStatusText = "Image uploaded. Click Save to keep this image for the package.";
        });
    }

    [RelayCommand(CanExecute = nameof(CanUseImageCommands))]
    private void ClearPackageImage()
    {
        if (SelectedPackage is null)
        {
            return;
        }

        SelectedLocalImagePath = string.Empty;
        SelectedImagePreviewSource = null;

        SelectedPackage.ImageUrl = string.Empty;

        IsModified = true;
        ImageStatusText = "Image cleared. Click Save to apply this change.";

        UpdateCommandStates();
    }

    [RelayCommand]
    private void AddSize()
    {
        if (!IsEditing || SelectedPackage is null)
        {
            return;
        }

        var newSize = new PackageSizeItemVM
        {
            Label = "New Size",
            Subtitle = string.Empty,
            PaxCount = 1,
            Price = 0
        };

        SelectedPackage.Sizes.Add(newSize);
        SelectedSize = newSize;

        IsModified = true;
        UpdateCommandStates();
    }

    [RelayCommand]
    private void RemoveSize(PackageSizeItemVM? size)
    {
        if (!IsEditing || SelectedPackage is null || size is null)
        {
            return;
        }

        SelectedPackage.Sizes.Remove(size);

        if (ReferenceEquals(SelectedSize, size))
        {
            SelectedSize = SelectedPackage.Sizes.FirstOrDefault();
        }

        IsModified = true;
        UpdateCommandStates();
    }

    [RelayCommand]
    private void AddAddon()
    {
        if (!IsEditing || SelectedPackage is null)
        {
            return;
        }

        SelectedPackage.Addons.Add(new PackageAddonItemVM
        {
            Name = "New Addon",
            Description = string.Empty,
            IsAvailable = true,
            Price = 0
        });

        IsModified = true;
    }

    [RelayCommand]
    private void RemoveAddon(PackageAddonItemVM? addon)
    {
        if (!IsEditing || SelectedPackage is null || addon is null)
        {
            return;
        }

        SelectedPackage.Addons.Remove(addon);
        IsModified = true;
    }

    [RelayCommand]
    private void AddSelectionRule()
    {
        if (!IsEditing || SelectedSize is null)
        {
            return;
        }

        SelectedSize.SelectionRules.Add(new PackageSelectionRuleItemVM
        {
            Title = "New Rule",
            Description = string.Empty,
            SelectionType = SelectionTypeOptions.FirstOrDefault() ?? string.Empty,
            AllowedMealType = MealTypeOptions.FirstOrDefault() ?? string.Empty,
            MinSelections = 1,
            MaxSelections = 1,
            IsRequired = true,
            DisplayOrder = SelectedSize.SelectionRules.Count + 1
        });

        IsModified = true;
    }

    [RelayCommand]
    private void RemoveSelectionRule(PackageSelectionRuleItemVM? rule)
    {
        if (!IsEditing || SelectedSize is null || rule is null)
        {
            return;
        }

        SelectedSize.SelectionRules.Remove(rule);
        IsModified = true;
    }

    [RelayCommand]
    private void AddOption(PackageSelectionRuleItemVM? rule)
    {
        if (!IsEditing || rule is null)
        {
            return;
        }

        rule.Options.Add(new PackageSelectionOptionItemVM
        {
            MealId = Meals.FirstOrDefault()?.Id ?? 0,
            MealName = Meals.FirstOrDefault()?.Name ?? string.Empty,
            AdditionalPrice = 0,
            IsDefault = false
        });

        IsModified = true;
    }

    [RelayCommand]
    private void RemoveOption(PackageSelectionOptionItemVM? option)
    {
        if (!IsEditing || SelectedSize is null || option is null)
        {
            return;
        }

        foreach (var rule in SelectedSize.SelectionRules)
        {
            if (rule.Options.Remove(option))
            {
                IsModified = true;
                return;
            }
        }
    }

    private async Task RunSafeAsync(Func<Task> action)
    {
        if (IsBusy)
        {
            return;
        }

        try
        {
            IsBusy = true;
            ErrorMessage = string.Empty;

            await action();
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.InnerException is not null
                ? $"{ex.Message}{Environment.NewLine}{ex.InnerException.Message}"
                : ex.Message;
        }
        finally
        {
            _isLoadingOrSaving = false;
            IsBusy = false;

            UpdateCommandStates();
        }
    }

    private void RefreshImagePreviewFromSelectedPackage()
    {
        SelectedLocalImagePath = string.Empty;

        if (SelectedPackage is null || string.IsNullOrWhiteSpace(SelectedPackage.ImageUrl))
        {
            SelectedImagePreviewSource = null;
            ImageStatusText = "No image uploaded.";
            return;
        }

        SelectedImagePreviewSource = CreateBitmapImage(SelectedPackage.ImageUrl);

        ImageStatusText = SelectedImagePreviewSource is null
            ? "Image URL could not be previewed."
            : "Existing image loaded from URL.";
    }

    private static BitmapImage? CreateBitmapImage(string source)
    {
        if (string.IsNullOrWhiteSpace(source))
        {
            return null;
        }

        try
        {
            var bitmap = new BitmapImage();

            bitmap.BeginInit();
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.UriSource = new Uri(source, UriKind.RelativeOrAbsolute);
            bitmap.EndInit();

            if (bitmap.CanFreeze)
            {
                bitmap.Freeze();
            }

            return bitmap;
        }
        catch
        {
            return null;
        }
    }

    private void UpdateCommandStates()
    {
        LoadCommand.NotifyCanExecuteChanged();
        NewPackageCommand.NotifyCanExecuteChanged();
        EditOrSaveCommand.NotifyCanExecuteChanged();
        DeleteCommand.NotifyCanExecuteChanged();

        UploadPackageImageFromDeviceCommand.NotifyCanExecuteChanged();
        ClearPackageImageCommand.NotifyCanExecuteChanged();
    }
}