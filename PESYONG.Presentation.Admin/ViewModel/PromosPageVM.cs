using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PESYONG.Presentation.Admin.Interfaces;

namespace PESYONG.Presentation.Admin.ViewModel;

public partial class PromosPageVM : ObservableObject
{
    private readonly IPromoApiService _promoApiService;

    public ObservableCollection<PromoItemVM> Promos { get; } = new();

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsFormEnabled))]
    private PromoItemVM? selectedPromo;

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(EditSaveButtonText))]
    [NotifyPropertyChangedFor(nameof(IsFormEnabled))]
    private bool isEditing;

    [ObservableProperty]
    private bool isModified;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasError))]
    private string errorMessage = string.Empty;

    public string EditSaveButtonText => IsEditing ? "Save" : "Edit";

    public bool IsFormEnabled => IsEditing && SelectedPromo is not null;

    public bool HasError => !string.IsNullOrWhiteSpace(ErrorMessage);

    public PromosPageVM(IPromoApiService promoApiService)
    {
        _promoApiService = promoApiService;
    }

    [RelayCommand(CanExecute = nameof(CanLoad))]
    private async Task LoadAsync()
    {
        try
        {
            IsBusy = true;
            ErrorMessage = string.Empty;

            Promos.Clear();

            var promos = await _promoApiService.GetAllAsync();

            foreach (var promo in promos)
            {
                Promos.Add(PromoItemVM.FromDto(promo));
            }

            SelectedPromo = Promos.FirstOrDefault();
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
            NotifyCommands();
        }
    }

    [RelayCommand(CanExecute = nameof(CanNewPromo))]
    private void NewPromo()
    {
        ErrorMessage = string.Empty;

        SelectedPromo = new PromoItemVM
        {
            ValidFromUtc = DateTime.UtcNow.Date,
            ValidUntilUtc = DateTime.UtcNow.Date.AddDays(30),
            DiscountPercentageValue = 0,
            UsedCount = 0
        };

        IsEditing = true;
        IsModified = false;

        NotifyCommands();
    }

    [RelayCommand(CanExecute = nameof(CanEditOrSave))]
    private async Task EditOrSaveAsync()
    {
        if (SelectedPromo is null)
            return;

        if (!IsEditing)
        {
            ErrorMessage = string.Empty;
            IsEditing = true;
            IsModified = false;
            NotifyCommands();
            return;
        }

        if (!ValidateSelectedPromo())
        {
            NotifyCommands();
            return;
        }

        try
        {
            IsBusy = true;
            ErrorMessage = string.Empty;

            if (SelectedPromo.Id == 0)
            {
                var createdDto = await _promoApiService.CreateAsync(SelectedPromo.ToCreateRequest());
                var createdVm = PromoItemVM.FromDto(createdDto);

                Promos.Add(createdVm);

                IsEditing = false;
                SelectedPromo = createdVm;
            }
            else
            {
                var updatedDto = await _promoApiService.UpdateAsync(
                    SelectedPromo.Id,
                    SelectedPromo.ToUpdateRequest());

                IsEditing = false;
                SelectedPromo.CopyFrom(updatedDto);
            }

            IsModified = false;
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsBusy = false;
            NotifyCommands();
        }
    }

    [RelayCommand(CanExecute = nameof(CanDelete))]
    private async Task DeleteAsync()
    {
        if (SelectedPromo is null || SelectedPromo.Id == 0)
            return;

        try
        {
            IsBusy = true;
            ErrorMessage = string.Empty;

            var promoToDelete = SelectedPromo;

            await _promoApiService.DeleteAsync(promoToDelete.Id);

            Promos.Remove(promoToDelete);

            SelectedPromo = Promos.FirstOrDefault();
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
            NotifyCommands();
        }
    }

    private bool ValidateSelectedPromo()
    {
        if (SelectedPromo is null)
        {
            ErrorMessage = "No promo is selected.";
            return false;
        }

        if (string.IsNullOrWhiteSpace(SelectedPromo.Code))
        {
            ErrorMessage = "Promo code is required.";
            return false;
        }

        if (SelectedPromo.DiscountPercentageValue < 0 || SelectedPromo.DiscountPercentageValue > 100)
        {
            ErrorMessage = "Discount percentage must be between 0 and 100.";
            return false;
        }

        if (SelectedPromo.MinimumOrderAmount.HasValue && SelectedPromo.MinimumOrderAmount < 0)
        {
            ErrorMessage = "Minimum order amount cannot be negative.";
            return false;
        }

        if (SelectedPromo.UsageLimit.HasValue && SelectedPromo.UsageLimit < 0)
        {
            ErrorMessage = "Usage limit cannot be negative.";
            return false;
        }

        if (SelectedPromo.UsedCount < 0)
        {
            ErrorMessage = "Used count cannot be negative.";
            return false;
        }

        if (!SelectedPromo.ValidFromUtc.HasValue)
        {
            ErrorMessage = "Valid From date is required.";
            return false;
        }

        if (!SelectedPromo.ValidUntilUtc.HasValue)
        {
            ErrorMessage = "Valid Until date is required.";
            return false;
        }

        if (SelectedPromo.ValidUntilUtc.Value < SelectedPromo.ValidFromUtc.Value)
        {
            ErrorMessage = "Valid Until must be later than or equal to Valid From.";
            return false;
        }

        ErrorMessage = string.Empty;
        return true;
    }

    private bool CanLoad() => !IsBusy;

    private bool CanNewPromo() => !IsBusy;

    private bool CanEditOrSave() => !IsBusy && SelectedPromo is not null;

    private bool CanDelete() => !IsBusy && SelectedPromo is not null && SelectedPromo.Id > 0;

    partial void OnSelectedPromoChanging(PromoItemVM? oldValue, PromoItemVM? newValue)
    {
        if (oldValue is not null)
        {
            oldValue.PropertyChanged -= SelectedPromo_PropertyChanged;
        }
    }

    partial void OnSelectedPromoChanged(PromoItemVM? oldValue, PromoItemVM? newValue)
    {
        if (newValue is not null)
        {
            newValue.PropertyChanged += SelectedPromo_PropertyChanged;
        }

        IsEditing = false;
        IsModified = false;

        NotifyCommands();
    }

    partial void OnIsBusyChanged(bool value)
    {
        NotifyCommands();
    }

    partial void OnIsEditingChanged(bool value)
    {
        NotifyCommands();
    }

    private void SelectedPromo_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (!IsEditing)
            return;

        IsModified = true;
        NotifyCommands();
    }

    private void NotifyCommands()
    {
        LoadCommand.NotifyCanExecuteChanged();
        NewPromoCommand.NotifyCanExecuteChanged();
        EditOrSaveCommand.NotifyCanExecuteChanged();
        DeleteCommand.NotifyCanExecuteChanged();
    }
}