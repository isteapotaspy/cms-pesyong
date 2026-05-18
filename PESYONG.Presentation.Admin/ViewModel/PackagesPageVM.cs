using System.Collections.ObjectModel;
using System.ComponentModel;
using CMS.Contracts.Admin.Package;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PESYONG.Presentation.Admin.Interfaces;

namespace PESYONG.Presentation.Admin.ViewModel;

public partial class PackagesPageVM : ObservableObject
{
    private readonly IPackageApiService _packageApiService;

    public PackagesPageVM(IPackageApiService packageApiService)
    {
        _packageApiService = packageApiService;
    }

    public ObservableCollection<PackageItemVM> Packages { get; } = new();
    public ObservableCollection<LookupItemDto> MenuCategories { get; } = new();
    public ObservableCollection<LookupItemDto> Meals { get; } = new();
    public ObservableCollection<string> SelectionTypeOptions { get; } = new();
    public ObservableCollection<string> MealTypeOptions { get; } = new();

    [ObservableProperty] private PackageItemVM? selectedPackage;
    [ObservableProperty] private bool isBusy;
    [ObservableProperty] private bool isEditing;
    [ObservableProperty] private bool isModified;
    [ObservableProperty] private string errorMessage = string.Empty;

    public string EditSaveButtonText => IsEditing ? "Save" : "Edit";

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

        if (!IsEditing)
        {
            IsModified = false;
        }

        UpdateCommandStates();
    }

    partial void OnIsBusyChanged(bool value)
    {
        UpdateCommandStates();
    }

    partial void OnIsEditingChanged(bool value)
    {
        OnPropertyChanged(nameof(EditSaveButtonText));
        UpdateCommandStates();
    }

    private void SelectedPackage_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (IsEditing)
        {
            IsModified = true;
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
        return !IsBusy && SelectedPackage is not null;
    }

    private bool CanDelete()
    {
        return !IsBusy &&
               !IsEditing &&
               SelectedPackage is not null &&
               SelectedPackage.Id > 0;
    }

    [RelayCommand(CanExecute = nameof(CanLoad))]
    private async Task LoadAsync()
    {
        try
        {
            IsBusy = true;
            ErrorMessage = string.Empty;
            IsEditing = false;

            await LoadLookupsAsync();

            var packages = await _packageApiService.GetAllAsync();

            Packages.Clear();
            foreach (var package in packages)
            {
                Packages.Add(PackageItemVM.FromDto(package));
            }

            SelectedPackage = Packages.FirstOrDefault();
            IsModified = false;
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
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
        SelectedPackage = new PackageItemVM
        {
            MenuCategoryId = MenuCategories.FirstOrDefault()?.Id ?? 0,
            Title = string.Empty,
            IsAvailable = true,
            IsCustomizable = false,
            Rating = 0,
            ReviewCount = 0
        };

        IsEditing = true;
        IsModified = true;
        ErrorMessage = string.Empty;
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
            return;
        }

        try
        {
            IsBusy = true;
            ErrorMessage = string.Empty;

            PackageDto savedPackage;

            if (SelectedPackage.Id == 0)
            {
                savedPackage = await _packageApiService.CreateAsync(SelectedPackage.ToCreateRequest());

                var newVm = PackageItemVM.FromDto(savedPackage);
                Packages.Insert(0, newVm);
                SelectedPackage = newVm;
            }
            else
            {
                savedPackage = await _packageApiService.UpdateAsync(
                    SelectedPackage.Id,
                    SelectedPackage.ToUpdateRequest());

                SelectedPackage.CopyFrom(savedPackage);
            }

            IsEditing = false;
            IsModified = false;
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand(CanExecute = nameof(CanDelete))]
    private async Task DeleteAsync()
    {
        if (SelectedPackage is null || SelectedPackage.Id == 0)
        {
            return;
        }

        try
        {
            IsBusy = true;
            ErrorMessage = string.Empty;

            var packageToDelete = SelectedPackage;

            await _packageApiService.DeleteAsync(packageToDelete.Id);

            Packages.Remove(packageToDelete);
            SelectedPackage = Packages.FirstOrDefault();
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private void AddSize()
    {
        if (!IsEditing || SelectedPackage is null)
        {
            return;
        }

        SelectedPackage.Sizes.Add(new PackageSizeItemVM
        {
            Label = "New Size",
            PaxCount = 1,
            Price = 0
        });

        IsModified = true;
    }

    [RelayCommand]
    private void RemoveSize(PackageSizeItemVM? size)
    {
        if (!IsEditing || SelectedPackage is null || size is null)
        {
            return;
        }

        SelectedPackage.Sizes.Remove(size);
        IsModified = true;
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
        if (!IsEditing || SelectedPackage is null)
        {
            return;
        }

        SelectedPackage.SelectionRules.Add(new PackageSelectionRuleItemVM
        {
            Title = "New Rule",
            SelectionType = SelectionTypeOptions.FirstOrDefault() ?? string.Empty,
            AllowedMealType = MealTypeOptions.FirstOrDefault() ?? string.Empty,
            MinSelections = 0,
            MaxSelections = 1,
            IsRequired = true,
            DisplayOrder = SelectedPackage.SelectionRules.Count + 1
        });

        IsModified = true;
    }

    [RelayCommand]
    private void RemoveSelectionRule(PackageSelectionRuleItemVM? rule)
    {
        if (!IsEditing || SelectedPackage is null || rule is null)
        {
            return;
        }

        SelectedPackage.SelectionRules.Remove(rule);
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
            AdditionalPrice = 0,
            IsDefault = false
        });

        IsModified = true;
    }

    [RelayCommand]
    private void RemoveOption(PackageSelectionOptionItemVM? option)
    {
        if (!IsEditing || SelectedPackage is null || option is null)
        {
            return;
        }

        foreach (var rule in SelectedPackage.SelectionRules)
        {
            if (rule.Options.Remove(option))
            {
                IsModified = true;
                return;
            }
        }
    }

    private void UpdateCommandStates()
    {
        LoadCommand.NotifyCanExecuteChanged();
        NewPackageCommand.NotifyCanExecuteChanged();
        EditOrSaveCommand.NotifyCanExecuteChanged();
        DeleteCommand.NotifyCanExecuteChanged();
    }
}