using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using PESYONG.Presentation.Admin.Interfaces;

namespace PESYONG.Presentation.Admin.ViewModel;

public partial class MealPageVM : ObservableObject
{
    private readonly IMealApiService _mealApiService;
    private readonly IImageApiService _imageApiService;

    private bool _isLoadingOrSaving;
    private bool _isSyncingCategory;
    private MealItemVM? _previousSelectedMeal;

    private const int DefaultCategoryId = 1;
    private const string DefaultMealType = "Kakanin";

    private static readonly IReadOnlyDictionary<int, string> CategoryNameById =
        new Dictionary<int, string>
        {
            [1] = "Authentic Kakanin",
            [2] = "Short Orders",
            [3] = "Catering"
        };

    private static readonly IReadOnlyDictionary<string, int> CategoryIdByName =
        CategoryNameById.ToDictionary(
            x => x.Value,
            x => x.Key,
            StringComparer.OrdinalIgnoreCase);

    public ObservableCollection<MealItemVM> Meals { get; } = new();

    public ObservableCollection<string> CategoryNameOptions { get; } =
        new(CategoryNameById
            .OrderBy(x => x.Key)
            .Select(x => x.Value));

    public ObservableCollection<string> MealTypeOptions { get; } =
    [
        "MainDish",
        "Viand",
        "SideDish",
        "Dessert",
        "Kakanin",
        "Beverage"
    ];

    [ObservableProperty]
    private MealItemVM selectedMeal = new();

    [ObservableProperty]
    private string selectedCategoryName = string.Empty;

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private bool isEditing;

    [ObservableProperty]
    private bool isModified;

    [ObservableProperty]
    private string errorMessage = string.Empty;

    [ObservableProperty]
    private string statusMessage = "Ready.";

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

    public MealPageVM(
        IMealApiService mealApiService,
        IImageApiService imageApiService)
    {
        _mealApiService = mealApiService;
        _imageApiService = imageApiService;
    }

    [RelayCommand(CanExecute = nameof(CanRunCommand))]
    private async Task LoadAsync()
    {
        await RunSafeAsync(async () =>
        {
            StatusMessage = "Loading meals...";
            Meals.Clear();

            var meals = await _mealApiService.GetMealsAsync();

            foreach (var meal in meals)
            {
                var mealVm = MealItemVM.FromDto(meal);

                NormalizeMealCategory(mealVm);
                NormalizeMealType(mealVm);

                Meals.Add(mealVm);
            }

            _isLoadingOrSaving = true;

            SelectedMeal = Meals.FirstOrDefault() ?? new MealItemVM();

            SyncSelectedCategoryNameFromSelectedMeal();

            IsEditing = false;
            IsModified = false;

            _isLoadingOrSaving = false;

            RefreshImagePreviewFromSelectedMeal();

            StatusMessage = Meals.Count == 0
                ? "No meals found."
                : $"{Meals.Count} meal(s) loaded.";
        });
    }

    [RelayCommand(CanExecute = nameof(CanRunCommand))]
    private void NewMeal()
    {
        _isLoadingOrSaving = true;

        SelectedMeal = new MealItemVM
        {
            CategoryId = DefaultCategoryId,
            CategoryName = CategoryNameById[DefaultCategoryId],

            Name = string.Empty,
            Description = string.Empty,
            MealType = DefaultMealType,

            BasePrice = 0,
            StockQuantity = 0,
            MinOrderQuantity = 1,

            ImageUrl = string.Empty,
            IsAvailable = true,
            IsViandOption = false
        };

        SyncSelectedCategoryNameFromSelectedMeal();

        _isLoadingOrSaving = false;

        SelectedLocalImagePath = string.Empty;
        SelectedImagePreviewSource = null;
        ImageStatusText = "No image selected.";

        IsEditing = true;
        IsModified = true;
        StatusMessage = "Creating a new meal.";

        NotifyCommands();
    }

    [RelayCommand(CanExecute = nameof(CanEditOrSave))]
    private async Task EditOrSaveAsync()
    {
        if (!IsEditing)
        {
            IsEditing = true;
            IsModified = false;
            StatusMessage = "Editing meal details.";

            NotifyCommands();
            return;
        }

        await RunSafeAsync(async () =>
        {
            if (!TryApplySelectedCategory(out var categoryError))
            {
                ErrorMessage = categoryError;
                StatusMessage = "Choose a valid category before saving.";
                return;
            }

            NormalizeMealType(SelectedMeal);

            if (!ValidateMealForSave(out var validationError))
            {
                ErrorMessage = validationError;
                StatusMessage = "Please fix the meal details before saving.";
                return;
            }

            _isLoadingOrSaving = true;

            if (SelectedMeal.Id == 0)
            {
                StatusMessage = "Creating meal...";

                var request = SelectedMeal.ToCreateRequest();

                var createdMeal = await _mealApiService.CreateMealAsync(request);

                var createdMealVm = MealItemVM.FromDto(createdMeal);

                NormalizeMealCategory(createdMealVm);
                NormalizeMealType(createdMealVm);

                Meals.Add(createdMealVm);
                SelectedMeal = createdMealVm;

                StatusMessage = "Meal created.";
            }
            else
            {
                StatusMessage = "Saving meal...";

                var request = SelectedMeal.ToUpdateRequest();

                var updatedMeal = await _mealApiService.UpdateMealAsync(
                    SelectedMeal.Id,
                    request);

                SelectedMeal.CopyFrom(updatedMeal);

                NormalizeMealCategory(SelectedMeal);
                NormalizeMealType(SelectedMeal);

                StatusMessage = "Meal saved.";
            }

            SyncSelectedCategoryNameFromSelectedMeal();

            IsEditing = false;
            IsModified = false;

            _isLoadingOrSaving = false;

            RefreshImagePreviewFromSelectedMeal();
        });
    }

    [RelayCommand(CanExecute = nameof(CanDelete))]
    private async Task DeleteAsync()
    {
        await RunSafeAsync(async () =>
        {
            StatusMessage = "Deleting meal...";

            var id = SelectedMeal.Id;

            await _mealApiService.DeleteMealAsync(id);

            var mealToRemove = Meals.FirstOrDefault(x => x.Id == id);

            if (mealToRemove is not null)
            {
                Meals.Remove(mealToRemove);
            }

            _isLoadingOrSaving = true;

            SelectedMeal = Meals.FirstOrDefault() ?? new MealItemVM();

            SyncSelectedCategoryNameFromSelectedMeal();

            IsEditing = false;
            IsModified = false;

            _isLoadingOrSaving = false;

            RefreshImagePreviewFromSelectedMeal();

            StatusMessage = "Meal deleted.";
        });
    }

    [RelayCommand(CanExecute = nameof(CanUseImageCommands))]
    private async Task UploadMealImageFromDeviceAsync()
    {
        var dialog = new OpenFileDialog
        {
            Title = "Choose meal image",
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

            SelectedMeal.ImageUrl = uploadedImage.ImageUrl;

            IsModified = true;
            ImageStatusText = "Image uploaded. Click Save to keep this image for the meal.";
        });
    }

    [RelayCommand(CanExecute = nameof(CanUseImageCommands))]
    private void ClearMealImage()
    {
        SelectedLocalImagePath = string.Empty;
        SelectedImagePreviewSource = null;

        SelectedMeal.ImageUrl = string.Empty;

        IsModified = true;
        ImageStatusText = "Image cleared. Click Save to apply this change.";

        NotifyCommands();
    }

    private void NormalizeMealCategory(MealItemVM meal)
    {
        if (meal.CategoryId > 0 &&
            CategoryNameById.TryGetValue(meal.CategoryId, out var categoryNameFromId))
        {
            meal.CategoryName = categoryNameFromId;
            return;
        }

        if (!string.IsNullOrWhiteSpace(meal.CategoryName) &&
            CategoryIdByName.TryGetValue(meal.CategoryName, out var categoryIdFromName))
        {
            meal.CategoryId = categoryIdFromName;
            meal.CategoryName = CategoryNameById[categoryIdFromName];
            return;
        }

        meal.CategoryId = 0;
        meal.CategoryName = string.Empty;
    }

    private void NormalizeMealType(MealItemVM meal)
    {
        if (string.IsNullOrWhiteSpace(meal.MealType))
        {
            meal.MealType = DefaultMealType;
            return;
        }

        var matchingMealType = MealTypeOptions.FirstOrDefault(
            x => string.Equals(x, meal.MealType.Trim(), StringComparison.OrdinalIgnoreCase));

        meal.MealType = matchingMealType ?? DefaultMealType;
    }

    private bool TryApplySelectedCategory(out string error)
    {
        error = string.Empty;

        if (string.IsNullOrWhiteSpace(SelectedCategoryName))
        {
            error = "Category is required.";
            return false;
        }

        if (!CategoryIdByName.TryGetValue(SelectedCategoryName, out var categoryId))
        {
            error = $"Invalid category: {SelectedCategoryName}.";
            return false;
        }

        ApplyCategoryById(categoryId);

        return true;
    }

    private void ApplyCategoryById(int categoryId)
    {
        if (!CategoryNameById.TryGetValue(categoryId, out var categoryName))
        {
            SelectedMeal.CategoryId = 0;
            SelectedMeal.CategoryName = string.Empty;
            return;
        }

        SelectedMeal.CategoryId = categoryId;
        SelectedMeal.CategoryName = categoryName;
    }

    private void ApplyCategoryByName(string? categoryName)
    {
        if (_isSyncingCategory)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(categoryName))
        {
            SelectedMeal.CategoryId = 0;
            SelectedMeal.CategoryName = string.Empty;
            return;
        }

        if (!CategoryIdByName.TryGetValue(categoryName, out var categoryId))
        {
            return;
        }

        ApplyCategoryById(categoryId);

        if (IsEditing && !_isLoadingOrSaving)
        {
            IsModified = true;
        }
    }

    private void SyncSelectedCategoryNameFromSelectedMeal()
    {
        try
        {
            _isSyncingCategory = true;

            if (SelectedMeal is null)
            {
                SelectedCategoryName = string.Empty;
                return;
            }

            NormalizeMealCategory(SelectedMeal);

            SelectedCategoryName =
                SelectedMeal.CategoryId > 0 &&
                CategoryNameById.TryGetValue(SelectedMeal.CategoryId, out var categoryName)
                    ? categoryName
                    : string.Empty;
        }
        finally
        {
            _isSyncingCategory = false;
        }
    }

    private bool ValidateMealForSave(out string error)
    {
        error = string.Empty;

        if (SelectedMeal is null)
        {
            error = "No meal selected.";
            return false;
        }

        if (SelectedMeal.CategoryId is not 1 and not 2 and not 3)
        {
            error = "Please select a valid category.";
            return false;
        }

        if (string.IsNullOrWhiteSpace(SelectedMeal.Name))
        {
            error = "Meal name is required.";
            return false;
        }

        if (!MealTypeOptions.Contains(SelectedMeal.MealType))
        {
            error = $"Invalid meal type: {SelectedMeal.MealType}.";
            return false;
        }

        if (SelectedMeal.BasePrice < 0)
        {
            error = "Base price cannot be negative.";
            return false;
        }

        if (SelectedMeal.StockQuantity < 0)
        {
            error = "Stock quantity cannot be negative.";
            return false;
        }

        if (SelectedMeal.MinOrderQuantity <= 0)
        {
            error = "Minimum order quantity must be at least 1.";
            return false;
        }

        return true;
    }

    private bool CanRunCommand()
    {
        return !IsBusy;
    }

    private bool CanEditOrSave()
    {
        if (IsBusy)
        {
            return false;
        }

        if (SelectedMeal is null)
        {
            return false;
        }

        if (!IsEditing)
        {
            return SelectedMeal.Id > 0;
        }

        return IsModified &&
               !string.IsNullOrWhiteSpace(SelectedCategoryName) &&
               CategoryIdByName.ContainsKey(SelectedCategoryName) &&
               !string.IsNullOrWhiteSpace(SelectedMeal.MealType);
    }

    private bool CanDelete()
    {
        return !IsBusy &&
               !IsEditing &&
               SelectedMeal is not null &&
               SelectedMeal.Id > 0;
    }

    private bool CanUseImageCommands()
    {
        return !IsBusy &&
               IsEditing &&
               SelectedMeal is not null;
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

            StatusMessage = "Action failed.";
        }
        finally
        {
            _isLoadingOrSaving = false;
            IsBusy = false;

            NotifyCommands();
        }
    }

    private void SelectedMeal_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (_isLoadingOrSaving)
        {
            return;
        }

        if (!IsEditing)
        {
            return;
        }

        IsModified = true;

        NotifyCommands();
    }

    partial void OnSelectedMealChanged(MealItemVM value)
    {
        if (_previousSelectedMeal is not null)
        {
            _previousSelectedMeal.PropertyChanged -= SelectedMeal_PropertyChanged;
        }

        if (value is not null)
        {
            value.PropertyChanged += SelectedMeal_PropertyChanged;
        }

        _previousSelectedMeal = value;

        if (!_isLoadingOrSaving)
        {
            IsEditing = false;
            IsModified = false;

            NormalizeMealType(value);
            SyncSelectedCategoryNameFromSelectedMeal();
            RefreshImagePreviewFromSelectedMeal();

            StatusMessage = value.Id > 0
                ? "Meal selected."
                : "Ready.";
        }

        NotifyCommands();
    }

    partial void OnSelectedCategoryNameChanged(string value)
    {
        if (_isSyncingCategory)
        {
            return;
        }

        ApplyCategoryByName(value);

        NotifyCommands();
    }

    partial void OnIsBusyChanged(bool value)
    {
        NotifyCommands();
    }

    partial void OnIsEditingChanged(bool value)
    {
        OnPropertyChanged(nameof(IsNotEditing));
        OnPropertyChanged(nameof(EditSaveButtonText));

        NotifyCommands();
    }

    partial void OnIsModifiedChanged(bool value)
    {
        NotifyCommands();
    }

    partial void OnSelectedImagePreviewSourceChanged(ImageSource? value)
    {
        OnPropertyChanged(nameof(ImagePlaceholderText));
    }

    private void RefreshImagePreviewFromSelectedMeal()
    {
        SelectedLocalImagePath = string.Empty;

        if (SelectedMeal is null || string.IsNullOrWhiteSpace(SelectedMeal.ImageUrl))
        {
            SelectedImagePreviewSource = null;
            ImageStatusText = "No image uploaded.";
            return;
        }

        SelectedImagePreviewSource = CreateBitmapImage(SelectedMeal.ImageUrl);
        ImageStatusText = "Existing image loaded from URL.";
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

    private void NotifyCommands()
    {
        LoadCommand.NotifyCanExecuteChanged();
        NewMealCommand.NotifyCanExecuteChanged();
        EditOrSaveCommand.NotifyCanExecuteChanged();
        DeleteCommand.NotifyCanExecuteChanged();

        UploadMealImageFromDeviceCommand.NotifyCanExecuteChanged();
        ClearMealImageCommand.NotifyCanExecuteChanged();
    }
}