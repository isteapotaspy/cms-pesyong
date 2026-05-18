using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using CMS.Domain.Enums;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PESYONG.Presentation.Admin.Interfaces;

namespace PESYONG.Presentation.Admin.ViewModel;

public partial class PaymentPageVM : ObservableObject
{
    private readonly IPaymentApiService _paymentApiService;

    public PaymentPageVM(IPaymentApiService paymentApiService)
    {
        _paymentApiService = paymentApiService;
    }

    public ObservableCollection<PaymentItemVM> Payments { get; } = new();

    public IReadOnlyList<string> PaymentMethods { get; } =
        Enum.GetNames(typeof(PaymentMethod));

    public IReadOnlyList<string> PaymentStatuses { get; } =
        Enum.GetNames(typeof(PaymentStatus));

    [ObservableProperty]
    private PaymentItemVM? selectedPayment;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(LoadingDetailsText))]
    [NotifyCanExecuteChangedFor(nameof(LoadCommand))]
    [NotifyCanExecuteChangedFor(nameof(NewPaymentCommand))]
    [NotifyCanExecuteChangedFor(nameof(EditOrSaveCommand))]
    [NotifyCanExecuteChangedFor(nameof(DeleteCommand))]
    private bool isBusy;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotEditing))]
    [NotifyPropertyChangedFor(nameof(EditSaveButtonText))]
    [NotifyCanExecuteChangedFor(nameof(NewPaymentCommand))]
    [NotifyCanExecuteChangedFor(nameof(EditOrSaveCommand))]
    [NotifyCanExecuteChangedFor(nameof(DeleteCommand))]
    private bool isEditing;

    [ObservableProperty]
    private bool isModified;

    [ObservableProperty]
    private string errorMessage = string.Empty;

    public bool IsNotEditing => !IsEditing;

    public string EditSaveButtonText => IsEditing ? "Save" : "Edit";

    public string LoadingDetailsText => IsBusy ? "Loading payment details..." : string.Empty;

    partial void OnSelectedPaymentChanging(PaymentItemVM? oldValue, PaymentItemVM? newValue)
    {
        if (oldValue is not null)
            oldValue.PropertyChanged -= SelectedPayment_PropertyChanged;
    }

    partial void OnSelectedPaymentChanged(PaymentItemVM? oldValue, PaymentItemVM? newValue)
    {
        if (newValue is not null)
            newValue.PropertyChanged += SelectedPayment_PropertyChanged;

        IsEditing = false;
        IsModified = false;
        ErrorMessage = string.Empty;

        EditOrSaveCommand.NotifyCanExecuteChanged();
        DeleteCommand.NotifyCanExecuteChanged();
    }

    private void SelectedPayment_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (IsEditing)
            IsModified = true;
    }

    private bool CanLoad()
    {
        return !IsBusy;
    }

    [RelayCommand(CanExecute = nameof(CanLoad))]
    private async Task LoadAsync()
    {
        try
        {
            IsBusy = true;
            ErrorMessage = string.Empty;

            Payments.Clear();

            var payments = await _paymentApiService.GetAllAsync();

            foreach (var payment in payments)
                Payments.Add(PaymentItemVM.FromDto(payment));

            SelectedPayment = Payments.FirstOrDefault();
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

    private bool CanNewPayment()
    {
        return !IsBusy && !IsEditing;
    }

    [RelayCommand(CanExecute = nameof(CanNewPayment))]
    private void NewPayment()
    {
        SelectedPayment = new PaymentItemVM
        {
            TimestampUtc = DateTime.UtcNow,
            PaymentMethod = PaymentMethods.FirstOrDefault() ?? string.Empty,
            PaymentStatus = PaymentStatuses.FirstOrDefault() ?? string.Empty
        };

        IsEditing = true;
        IsModified = true;
        ErrorMessage = string.Empty;
    }

    private bool CanEditOrSave()
    {
        return !IsBusy && SelectedPayment is not null;
    }

    [RelayCommand(CanExecute = nameof(CanEditOrSave))]
    private async Task EditOrSaveAsync()
    {
        if (SelectedPayment is null)
            return;

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

            if (SelectedPayment.Id == 0)
            {
                var createdDto = await _paymentApiService.CreateAsync(SelectedPayment.ToCreateRequest());
                var createdVm = PaymentItemVM.FromDto(createdDto);

                Payments.Add(createdVm);
                SelectedPayment = createdVm;
            }
            else
            {
                var updatedDto = await _paymentApiService.UpdateAsync(
                    SelectedPayment.Id,
                    SelectedPayment.ToUpdateRequest());

                var updatedVm = PaymentItemVM.FromDto(updatedDto);
                SelectedPayment.CopyFrom(updatedVm);
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

    private bool CanDelete()
    {
        return !IsBusy &&
               !IsEditing &&
               SelectedPayment is not null &&
               SelectedPayment.Id > 0;
    }

    [RelayCommand(CanExecute = nameof(CanDelete))]
    private async Task DeleteAsync()
    {
        if (SelectedPayment is null || SelectedPayment.Id == 0)
            return;

        try
        {
            IsBusy = true;
            ErrorMessage = string.Empty;

            var paymentToDelete = SelectedPayment;

            await _paymentApiService.DeleteAsync(paymentToDelete.Id);

            Payments.Remove(paymentToDelete);
            SelectedPayment = Payments.FirstOrDefault();
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
}