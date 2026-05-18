using System.Collections.ObjectModel;
using CMS.Domain.Enums;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PESYONG.Presentation.Admin.Interfaces;
using PESYONG.Presentation.Admin.Services;

namespace PESYONG.Presentation.Admin.ViewModels.Deliveries;

public partial class DeliveryPageVM : ObservableObject
{
    private readonly IDeliveryApiService _deliveryApiService;

    public DeliveryPageVM(IDeliveryApiService deliveryApiService)
    {
        _deliveryApiService = deliveryApiService;
    }

    public ObservableCollection<DeliveryItemVM> Deliveries { get; } = new();

    public IReadOnlyList<DeliveryStatus> DeliveryStatusOptions { get; } =
        Enum.GetValues<DeliveryStatus>();

    [ObservableProperty]
    private DeliveryItemVM? selectedDelivery;

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private bool isEditing;

    [ObservableProperty]
    private bool isModified;

    [ObservableProperty]
    private string errorMessage = string.Empty;

    public string EditSaveButtonText => IsEditing ? "Save" : "Edit";

    [RelayCommand(CanExecute = nameof(CanLoad))]
    private async Task LoadAsync()
    {
        try
        {
            IsBusy = true;
            ErrorMessage = string.Empty;

            Deliveries.Clear();

            var deliveries = await _deliveryApiService.GetAllAsync();

            foreach (var delivery in deliveries)
                Deliveries.Add(DeliveryItemVM.FromDto(delivery));

            SelectedDelivery = Deliveries.FirstOrDefault();
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

    private bool CanLoad()
    {
        return !IsBusy;
    }

    [RelayCommand(CanExecute = nameof(CanNewDelivery))]
    private void NewDelivery()
    {
        SelectedDelivery = new DeliveryItemVM
        {
            Status = DeliveryStatus.Pending
        };

        ErrorMessage = string.Empty;
        IsEditing = true;
        IsModified = true;
    }

    private bool CanNewDelivery()
    {
        return !IsBusy;
    }

    [RelayCommand(CanExecute = nameof(CanEditOrSave))]
    private async Task EditOrSaveAsync()
    {
        if (SelectedDelivery is null)
            return;

        if (!IsEditing)
        {
            IsEditing = true;
            ErrorMessage = string.Empty;
            return;
        }

        try
        {
            IsBusy = true;
            ErrorMessage = string.Empty;

            if (SelectedDelivery.Id == 0)
            {
                var created = await _deliveryApiService.CreateAsync(SelectedDelivery.ToCreateRequest());
                var createdVm = DeliveryItemVM.FromDto(created);

                Deliveries.Add(createdVm);
                SelectedDelivery = createdVm;
            }
            else
            {
                var updated = await _deliveryApiService.UpdateAsync(
                    SelectedDelivery.Id,
                    SelectedDelivery.ToUpdateRequest());

                SelectedDelivery.CopyFrom(updated);
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

    private bool CanEditOrSave()
    {
        return !IsBusy && SelectedDelivery is not null;
    }

    [RelayCommand(CanExecute = nameof(CanDelete))]
    private async Task DeleteAsync()
    {
        if (SelectedDelivery is null)
            return;

        try
        {
            IsBusy = true;
            ErrorMessage = string.Empty;

            if (SelectedDelivery.Id > 0)
                await _deliveryApiService.DeleteAsync(SelectedDelivery.Id);

            Deliveries.Remove(SelectedDelivery);
            SelectedDelivery = Deliveries.FirstOrDefault();

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

    private bool CanDelete()
    {
        return !IsBusy && SelectedDelivery is not null;
    }

    partial void OnSelectedDeliveryChanging(DeliveryItemVM? oldValue, DeliveryItemVM? newValue)
    {
        if (oldValue is not null)
            oldValue.PropertyChanged -= SelectedDelivery_PropertyChanged;
    }

    partial void OnSelectedDeliveryChanged(DeliveryItemVM? oldValue, DeliveryItemVM? newValue)
    {
        if (newValue is not null)
            newValue.PropertyChanged += SelectedDelivery_PropertyChanged;

        IsEditing = false;
        IsModified = false;
        NotifyCommands();
    }

    private void SelectedDelivery_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (IsEditing)
            IsModified = true;
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

    private void NotifyCommands()
    {
        LoadCommand.NotifyCanExecuteChanged();
        NewDeliveryCommand.NotifyCanExecuteChanged();
        EditOrSaveCommand.NotifyCanExecuteChanged();
        DeleteCommand.NotifyCanExecuteChanged();
    }
}