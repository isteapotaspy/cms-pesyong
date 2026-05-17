using System.Collections.ObjectModel;
using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PESYONG.Presentation.Admin.Interfaces;
using PESYONG.Presentation.Admin.Services;

namespace PESYONG.Presentation.Admin.ViewModels.Meals;

public partial class MealPageVM : ObservableObject
{
    private readonly IMealApiService _mealApiService;

    private bool _isLoadingOrSaving;
    private MealItemVM? _previousSelectedMeal;

    public ObservableCollection<MealItemVM> Meals { get; } = new();

    [ObservableProperty]
    private MealItemVM selectedMeal = new();

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private bool isEditing;

    [ObservableProperty]
    private bool isModified;

    [ObservableProperty]
    private string errorMessage = string.Empty;

    public string EditSaveButtonText => IsEditing ? "Save" : "Edit";

    public MealPageVM(IMealApiService mealApiService)
    {
        _mealApiService = mealApiService;
    }

    [RelayCommand(CanExecute = nameof(CanRunCommand))]
    private async Task LoadAsync()
    {
        await RunSafeAsync(async () =>
        {
            Meals.Clear();

            var meals = await _mealApiService.GetMealsAsync();

            foreach (var meal in meals)
            {
                Meals.Add(MealItemVM.FromDto(meal));
            }

            _isLoadingOrSaving = true;

            SelectedMeal = Meals.FirstOrDefault() ?? new MealItemVM();

            IsEditing = false;
            IsModified = false;

            _isLoadingOrSaving = false;
        });
    }

    [RelayCommand(CanExecute = nameof(CanRunCommand))]
    private void NewMeal()
    {
        SelectedMeal = new MealItemVM
        {
            MinOrderQuantity = 1,
            IsAvailable = true
        };

        IsEditing = true;
        IsModified = true;

        NotifyCommands();
    }

    [RelayCommand(CanExecute = nameof(CanEditOrSave))]
    private async Task EditOrSaveAsync()
    {
        if (!IsEditing)
        {
            IsEditing = true;
            IsModified = false;

            NotifyCommands();
            return;
        }

        await RunSafeAsync(async () =>
        {
            _isLoadingOrSaving = true;

            if (SelectedMeal.Id == 0)
            {
                var request = SelectedMeal.ToCreateRequest();

                var createdMeal = await _mealApiService.CreateMealAsync(request);

                var createdMealVm = MealItemVM.FromDto(createdMeal);

                Meals.Add(createdMealVm);
                SelectedMeal = createdMealVm;
            }
            else
            {
                var request = SelectedMeal.ToUpdateRequest();

                var updatedMeal = await _mealApiService.UpdateMealAsync(
                    SelectedMeal.Id,
                    request);

                SelectedMeal.CopyFrom(updatedMeal);
            }

            IsEditing = false;
            IsModified = false;

            _isLoadingOrSaving = false;
        });
    }

    [RelayCommand(CanExecute = nameof(CanDelete))]
    private async Task DeleteAsync()
    {
        await RunSafeAsync(async () =>
        {
            var id = SelectedMeal.Id;

            await _mealApiService.DeleteMealAsync(id);

            var mealToRemove = Meals.FirstOrDefault(x => x.Id == id);

            if (mealToRemove is not null)
            {
                Meals.Remove(mealToRemove);
            }

            SelectedMeal = Meals.FirstOrDefault() ?? new MealItemVM();

            IsEditing = false;
            IsModified = false;
        });
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

        return IsModified;
    }

    private bool CanDelete()
    {
        return !IsBusy &&
               !IsEditing &&
               SelectedMeal is not null &&
               SelectedMeal.Id > 0;
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
            ErrorMessage = ex.Message;
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
        }

        NotifyCommands();
    }

    partial void OnIsBusyChanged(bool value)
    {
        NotifyCommands();
    }

    partial void OnIsEditingChanged(bool value)
    {
        OnPropertyChanged(nameof(EditSaveButtonText));
        NotifyCommands();
    }

    partial void OnIsModifiedChanged(bool value)
    {
        NotifyCommands();
    }

    private void NotifyCommands()
    {
        LoadCommand.NotifyCanExecuteChanged();
        NewMealCommand.NotifyCanExecuteChanged();
        EditOrSaveCommand.NotifyCanExecuteChanged();
        DeleteCommand.NotifyCanExecuteChanged();
    }
}