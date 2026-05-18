using System.Collections.ObjectModel;
using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PESYONG.Presentation.Admin.Interfaces;
using PESYONG.Presentation.Admin.ViewModels.Orders;

namespace PESYONG.Presentation.Admin.ViewModels;

public partial class OrderPageVM : ObservableObject
{
    private readonly IOrderApiService _orderApiService;

    private bool _isSavingOrLoading;

    public ObservableCollection<OrderItemVM> Orders { get; } = new();

    public IReadOnlyList<string> OrderStatuses { get; } =
    [
        "Pending",
        "Confirmed",
        "Preparing",
        "ReadyForDelivery",
        "OutForDelivery",
        "Completed",
        "Cancelled"
    ];

    public IReadOnlyList<string> PaymentStatuses { get; } =
    [
        "Pending",
        "Paid",
        "Failed",
        "Refunded",
        "Cancelled"
    ];

    public IReadOnlyList<string> PaymentMethods { get; } =
    [
        "CashOnDelivery",
        "Cash",
        "GCash",
        "BankTransfer",
        "Card"
    ];

    public IReadOnlyList<string> OrderItemTypes { get; } =
    [
        "Package",
        "Meal"
    ];

    [ObservableProperty]
    private OrderItemVM? selectedOrder;

    [ObservableProperty]
    private OrderLineItemVM? selectedOrderLineItem;

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private bool isEditing;

    [ObservableProperty]
    private bool isModified;

    [ObservableProperty]
    private string errorMessage = string.Empty;

    [ObservableProperty]
    private string editSaveButtonText = "Edit";

    public OrderPageVM(IOrderApiService orderApiService)
    {
        _orderApiService = orderApiService;
    }

    partial void OnSelectedOrderChanging(OrderItemVM? oldValue, OrderItemVM? newValue)
    {
        if (oldValue is not null)
            oldValue.PropertyChanged -= SelectedOrder_PropertyChanged;
    }

    partial void OnSelectedOrderChanged(OrderItemVM? value)
    {
        if (value is not null)
            value.PropertyChanged += SelectedOrder_PropertyChanged;

        SelectedOrderLineItem = value?.Items.FirstOrDefault();

        RefreshCommandStates();
    }

    partial void OnIsBusyChanged(bool value)
    {
        RefreshCommandStates();
    }

    partial void OnIsEditingChanged(bool value)
    {
        EditSaveButtonText = value ? "Save" : "Edit";
        RefreshCommandStates();
    }

    private void SelectedOrder_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (!_isSavingOrLoading && IsEditing)
            IsModified = true;
    }

    [RelayCommand(CanExecute = nameof(CanLoad))]
    private async Task LoadAsync()
    {
        try
        {
            _isSavingOrLoading = true;
            IsBusy = true;
            ErrorMessage = string.Empty;

            var orders = await _orderApiService.GetAllAsync();

            Orders.Clear();

            foreach (var order in orders)
                Orders.Add(OrderItemVM.FromDto(order));

            SelectedOrder = Orders.FirstOrDefault();
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
            _isSavingOrLoading = false;
        }
    }

    private bool CanLoad()
    {
        return !IsBusy;
    }

    [RelayCommand(CanExecute = nameof(CanNewOrder))]
    private void NewOrder()
    {
        ErrorMessage = string.Empty;

        SelectedOrder = new OrderItemVM
        {
            OrderedAtUtc = DateTime.UtcNow,
            DeliveryDate = DateTime.Today,
            Status = "Pending",
            PaymentStatus = "Pending"
        };

        SelectedOrderLineItem = null;
        IsEditing = true;
        IsModified = true;
    }

    private bool CanNewOrder()
    {
        return !IsBusy;
    }

    [RelayCommand(CanExecute = nameof(CanEditOrSave))]
    private async Task EditOrSaveAsync()
    {
        if (SelectedOrder is null)
            return;

        if (!IsEditing)
        {
            IsEditing = true;
            IsModified = false;
            return;
        }

        try
        {
            _isSavingOrLoading = true;
            IsBusy = true;
            ErrorMessage = string.Empty;

            if (SelectedOrder.Id == 0)
            {
                var created = await _orderApiService.CreateAsync(SelectedOrder.ToCreateRequest());
                var createdVm = OrderItemVM.FromDto(created);

                Orders.Insert(0, createdVm);
                SelectedOrder = createdVm;
            }
            else
            {
                var updated = await _orderApiService.UpdateAsync(
                    SelectedOrder.Id,
                    SelectedOrder.ToUpdateRequest());

                SelectedOrder.CopyFrom(updated);
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
            _isSavingOrLoading = false;
        }
    }

    private bool CanEditOrSave()
    {
        return !IsBusy && SelectedOrder is not null;
    }

    [RelayCommand(CanExecute = nameof(CanDelete))]
    private async Task DeleteAsync()
    {
        if (SelectedOrder is null || SelectedOrder.Id == 0)
            return;

        try
        {
            IsBusy = true;
            ErrorMessage = string.Empty;

            var id = SelectedOrder.Id;

            await _orderApiService.DeleteAsync(id);

            var itemToRemove = Orders.FirstOrDefault(x => x.Id == id);
            if (itemToRemove is not null)
                Orders.Remove(itemToRemove);

            SelectedOrder = Orders.FirstOrDefault();
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
        return !IsBusy && !IsEditing && SelectedOrder is not null && SelectedOrder.Id > 0;
    }

    private void RefreshCommandStates()
    {
        LoadCommand.NotifyCanExecuteChanged();
        NewOrderCommand.NotifyCanExecuteChanged();
        EditOrSaveCommand.NotifyCanExecuteChanged();
        DeleteCommand.NotifyCanExecuteChanged();
    }
}