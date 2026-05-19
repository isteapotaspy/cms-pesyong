using System.Collections.ObjectModel;
using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PESYONG.Presentation.Admin.Interfaces;
using PESYONG.Presentation.Admin.ViewModels.Orders;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using Microsoft.Win32;

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

    [RelayCommand(CanExecute = nameof(CanPrintReceipt))]
    private async Task PrintReceiptAsync()
    {
        if (SelectedOrder is null)
            return;

        try
        {
            ErrorMessage = string.Empty;

            var orderNumber = string.IsNullOrWhiteSpace(SelectedOrder.OrderNumber)
                ? $"Order-{SelectedOrder.Id}"
                : SelectedOrder.OrderNumber;

            var safeOrderNumber = MakeSafeFileName(orderNumber);

            var dialog = new SaveFileDialog
            {
                Title = "Download Receipt",
                Filter = "HTML Receipt (*.html)|*.html|Text Receipt (*.txt)|*.txt",
                FileName = $"Receipt-{safeOrderNumber}.html",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };

            if (dialog.ShowDialog() != true)
                return;

            var extension = Path.GetExtension(dialog.FileName);

            var content = extension.Equals(".txt", StringComparison.OrdinalIgnoreCase)
                ? BuildReceiptText(SelectedOrder)
                : BuildReceiptHtml(SelectedOrder);

            await File.WriteAllTextAsync(dialog.FileName, content, Encoding.UTF8);

            Process.Start(new ProcessStartInfo
            {
                FileName = dialog.FileName,
                UseShellExecute = true
            });
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
    }

    private bool CanPrintReceipt()
    {
        return !IsBusy && !IsEditing && SelectedOrder is not null && SelectedOrder.Id > 0;
    }

    private void RefreshCommandStates()
    {
        LoadCommand.NotifyCanExecuteChanged();
        NewOrderCommand.NotifyCanExecuteChanged();
        EditOrSaveCommand.NotifyCanExecuteChanged();
        DeleteCommand.NotifyCanExecuteChanged();
        PrintReceiptCommand.NotifyCanExecuteChanged();
    }



    //=== HELPERS ===//

    private static string BuildReceiptHtml(OrderItemVM order)
    {
        var sb = new StringBuilder();

        sb.AppendLine("""
<!DOCTYPE html>
<html>
<head>
<meta charset="utf-8">
<title>Pesyong Receipt</title>
<style>
    body {
        font-family: Arial, sans-serif;
        background: #f7f1ec;
        color: #2b1a12;
        margin: 0;
        padding: 32px;
    }

    .receipt {
        max-width: 760px;
        margin: 0 auto;
        background: white;
        border-radius: 18px;
        padding: 32px;
        border: 1px solid #efd8c8;
        box-shadow: 0 18px 50px rgba(80, 45, 20, 0.14);
    }

    .header {
        display: flex;
        justify-content: space-between;
        gap: 24px;
        border-bottom: 2px solid #f05a28;
        padding-bottom: 18px;
        margin-bottom: 24px;
    }

    .brand {
        font-size: 30px;
        font-weight: 900;
        color: #f05a28;
    }

    .subtitle {
        color: #795b4b;
        margin-top: 4px;
    }

    .receipt-title {
        text-align: right;
        font-size: 18px;
        font-weight: 800;
    }

    .section {
        margin-top: 22px;
    }

    .section-title {
        font-size: 14px;
        font-weight: 900;
        color: #f05a28;
        text-transform: uppercase;
        letter-spacing: .06em;
        margin-bottom: 10px;
    }

    .info-grid {
        display: grid;
        grid-template-columns: 150px 1fr;
        gap: 8px 16px;
        font-size: 14px;
    }

    .label {
        color: #806353;
        font-weight: 700;
    }

    table {
        width: 100%;
        border-collapse: collapse;
        margin-top: 8px;
        font-size: 14px;
    }

    th {
        text-align: left;
        background: #fff3ec;
        color: #573524;
        padding: 10px;
        border-bottom: 1px solid #efd8c8;
    }

    td {
        padding: 10px;
        border-bottom: 1px solid #f0dfd3;
        vertical-align: top;
    }

    .right {
        text-align: right;
    }

    .totals {
        width: 320px;
        margin-left: auto;
        margin-top: 18px;
    }

    .total-row {
        display: flex;
        justify-content: space-between;
        padding: 7px 0;
        border-bottom: 1px solid #f0dfd3;
    }

    .grand-total {
        font-size: 20px;
        font-weight: 900;
        color: #f05a28;
        border-bottom: none;
        padding-top: 12px;
    }

    .notes {
        white-space: pre-wrap;
        line-height: 1.5;
        color: #5f4a3f;
    }

    .footer {
        margin-top: 28px;
        text-align: center;
        color: #8a6f60;
        font-size: 13px;
    }

    @media print {
        body {
            background: white;
            padding: 0;
        }

        .receipt {
            box-shadow: none;
            border: none;
        }
    }
</style>
</head>
<body>
<div class="receipt">
""");

        sb.AppendLine($"""
    <div class="header">
        <div>
            <div class="brand">Pesyong</div>
            <div class="subtitle">Official Order Receipt</div>
        </div>
        <div class="receipt-title">
            Receipt<br>
            {Html(order.OrderNumber)}
        </div>
    </div>
""");

        sb.AppendLine("""
    <div class="section">
        <div class="section-title">Order Information</div>
        <div class="info-grid">
""");

        AddInfo(sb, "Order ID", order.Id.ToString(CultureInfo.InvariantCulture));
        AddInfo(sb, "Order Number", order.OrderNumber);
        AddInfo(sb, "Ordered At", FormatDateTime(order.OrderedAtUtc));
        AddInfo(sb, "Delivery Date", FormatDate(order.DeliveryDate));
        AddInfo(sb, "Time Slot", order.DeliveryTimeSlot);
        AddInfo(sb, "Status", order.Status);
        AddInfo(sb, "Payment Method", order.PaymentMethod);
        AddInfo(sb, "Payment Status", order.PaymentStatus);

        sb.AppendLine("""
        </div>
    </div>
""");

        sb.AppendLine("""
    <div class="section">
        <div class="section-title">Customer Information</div>
        <div class="info-grid">
""");

        AddInfo(sb, "Customer ID", Convert.ToString(order.CustomerProfileId, CultureInfo.InvariantCulture));
        AddInfo(sb, "Address ID", Convert.ToString(order.AddressId, CultureInfo.InvariantCulture));
        AddInfo(sb, "Contact Name", order.ContactNameSnapshot);
        AddInfo(sb, "Contact Email", order.ContactEmailSnapshot);
        AddInfo(sb, "Contact Mobile", order.ContactMobileSnapshot);

        sb.AppendLine("""
        </div>
    </div>
""");

        sb.AppendLine("""
    <div class="section">
        <div class="section-title">Items</div>
        <table>
            <thead>
                <tr>
                    <th>Item</th>
                    <th>Type</th>
                    <th>Size</th>
                    <th class="right">Qty</th>
                    <th class="right">Unit</th>
                    <th class="right">Total</th>
                </tr>
            </thead>
            <tbody>
""");

        foreach (var item in order.Items)
        {
            var itemName = string.IsNullOrWhiteSpace(item.PackageTitleSnapshot)
                ? $"Item #{item.Id}"
                : item.PackageTitleSnapshot;

            sb.AppendLine("                <tr>");
            sb.AppendLine($"                    <td>{Html(itemName)}{BuildSelectionsHtml(item)}</td>");
            sb.AppendLine($"                    <td>{Html(item.ItemType)}</td>");
            sb.AppendLine($"                    <td>{Html(item.SizeLabelSnapshot)}</td>");
            sb.AppendLine($"                    <td class=\"right\">{item.Quantity}</td>");
            sb.AppendLine($"                    <td class=\"right\">{Money(item.UnitPrice)}</td>");
            sb.AppendLine($"                    <td class=\"right\">{Money(item.LineTotal)}</td>");
            sb.AppendLine("                </tr>");
        }

        sb.AppendLine("""
            </tbody>
        </table>
    </div>
""");

        sb.AppendLine("""
    <div class="totals">
""");

        AddTotal(sb, "Subtotal", order.SubTotal);
        AddTotal(sb, "Delivery Fee", order.DeliveryFee);
        AddTotal(sb, "Tax", order.TaxAmount);
        AddTotal(sb, "Discount", -order.DiscountAmount);

        if (!string.IsNullOrWhiteSpace(order.PromoCodeApplied))
        {
            sb.AppendLine($"""
        <div class="total-row">
            <span>Promo Code</span>
            <span>{Html(order.PromoCodeApplied)}</span>
        </div>
""");
        }

        sb.AppendLine($"""
        <div class="total-row grand-total">
            <span>Grand Total</span>
            <span>{Money(order.GrandTotal)}</span>
        </div>
    </div>
""");

        if (!string.IsNullOrWhiteSpace(order.CustomerNotes) ||
            !string.IsNullOrWhiteSpace(order.SpecialInstructions))
        {
            sb.AppendLine("""
    <div class="section">
        <div class="section-title">Notes</div>
""");

            if (!string.IsNullOrWhiteSpace(order.CustomerNotes))
            {
                sb.AppendLine($"""
        <div class="notes">
            <strong>Customer Notes:</strong><br>
            {Html(order.CustomerNotes)}
        </div>
""");
            }

            if (!string.IsNullOrWhiteSpace(order.SpecialInstructions))
            {
                sb.AppendLine($"""
        <div class="notes">
            <strong>Special Instructions:</strong><br>
            {Html(order.SpecialInstructions)}
        </div>
""");
            }

            sb.AppendLine("    </div>");
        }

        sb.AppendLine("""
    <div class="footer">
        Thank you for choosing Pesyong.
    </div>
</div>
</body>
</html>
""");

        return sb.ToString();
    }

    private static string BuildReceiptText(OrderItemVM order)
    {
        var sb = new StringBuilder();

        sb.AppendLine("PESYONG OFFICIAL ORDER RECEIPT");
        sb.AppendLine("========================================");
        sb.AppendLine($"Order Number: {order.OrderNumber}");
        sb.AppendLine($"Order ID: {order.Id}");
        sb.AppendLine($"Ordered At: {FormatDateTime(order.OrderedAtUtc)}");
        sb.AppendLine($"Delivery Date: {FormatDate(order.DeliveryDate)}");
        sb.AppendLine($"Time Slot: {order.DeliveryTimeSlot}");
        sb.AppendLine($"Status: {order.Status}");
        sb.AppendLine($"Payment Method: {order.PaymentMethod}");
        sb.AppendLine($"Payment Status: {order.PaymentStatus}");
        sb.AppendLine();

        sb.AppendLine("CUSTOMER");
        sb.AppendLine("----------------------------------------");
        sb.AppendLine($"Customer ID: {order.CustomerProfileId}");
        sb.AppendLine($"Address ID: {order.AddressId}");
        sb.AppendLine($"Name: {order.ContactNameSnapshot}");
        sb.AppendLine($"Email: {order.ContactEmailSnapshot}");
        sb.AppendLine($"Mobile: {order.ContactMobileSnapshot}");
        sb.AppendLine();

        sb.AppendLine("ITEMS");
        sb.AppendLine("----------------------------------------");

        foreach (var item in order.Items)
        {
            var itemName = string.IsNullOrWhiteSpace(item.PackageTitleSnapshot)
                ? $"Item #{item.Id}"
                : item.PackageTitleSnapshot;

            sb.AppendLine(itemName);
            sb.AppendLine($"Type: {item.ItemType}");
            sb.AppendLine($"Size: {item.SizeLabelSnapshot}");
            sb.AppendLine($"Qty: {item.Quantity}");
            sb.AppendLine($"Unit Price: {Money(item.UnitPrice)}");
            sb.AppendLine($"Line Total: {Money(item.LineTotal)}");

            foreach (var meal in item.MealSelections)
            {
                sb.AppendLine($"  Meal: {meal.MealNameSnapshot} ({Money(meal.AdditionalPrice)})");
            }

            foreach (var addon in item.AddonSelections)
            {
                sb.AppendLine($"  Addon: {addon.AddonNameSnapshot} ({Money(addon.AdditionalPrice)})");
            }

            sb.AppendLine();
        }

        sb.AppendLine("TOTALS");
        sb.AppendLine("----------------------------------------");
        sb.AppendLine($"Subtotal: {Money(order.SubTotal)}");
        sb.AppendLine($"Delivery Fee: {Money(order.DeliveryFee)}");
        sb.AppendLine($"Tax: {Money(order.TaxAmount)}");
        sb.AppendLine($"Discount: {Money(order.DiscountAmount)}");

        if (!string.IsNullOrWhiteSpace(order.PromoCodeApplied))
            sb.AppendLine($"Promo Code: {order.PromoCodeApplied}");

        sb.AppendLine($"Grand Total: {Money(order.GrandTotal)}");
        sb.AppendLine();

        if (!string.IsNullOrWhiteSpace(order.CustomerNotes))
        {
            sb.AppendLine("Customer Notes:");
            sb.AppendLine(order.CustomerNotes);
            sb.AppendLine();
        }

        if (!string.IsNullOrWhiteSpace(order.SpecialInstructions))
        {
            sb.AppendLine("Special Instructions:");
            sb.AppendLine(order.SpecialInstructions);
            sb.AppendLine();
        }

        sb.AppendLine("Thank you for choosing Pesyong.");

        return sb.ToString();
    }

    private static string BuildSelectionsHtml(OrderLineItemVM item)
    {
        var sb = new StringBuilder();

        if (item.MealSelections.Any() || item.AddonSelections.Any())
        {
            sb.AppendLine("<div style=\"margin-top:6px;color:#7d6254;font-size:12px;line-height:1.5;\">");

            foreach (var meal in item.MealSelections)
            {
                sb.Append($"Meal: {Html(meal.MealNameSnapshot)}");

                if (meal.AdditionalPrice != 0)
                    sb.Append($" ({Money(meal.AdditionalPrice)})");

                sb.AppendLine("<br>");
            }

            foreach (var addon in item.AddonSelections)
            {
                sb.Append($"Addon: {Html(addon.AddonNameSnapshot)}");

                if (addon.AdditionalPrice != 0)
                    sb.Append($" ({Money(addon.AdditionalPrice)})");

                sb.AppendLine("<br>");
            }

            sb.AppendLine("</div>");
        }

        return sb.ToString();
    }

    private static void AddInfo(StringBuilder sb, string label, string? value)
    {
        sb.AppendLine($"            <div class=\"label\">{Html(label)}</div>");
        sb.AppendLine($"            <div>{Html(string.IsNullOrWhiteSpace(value) ? "-" : value)}</div>");
    }

    private static void AddTotal(StringBuilder sb, string label, decimal amount)
    {
        sb.AppendLine($"""
        <div class="total-row">
            <span>{Html(label)}</span>
            <span>{Money(amount)}</span>
        </div>
        """);
    }

    private static string Money(decimal amount)
    {
        return $"₱{amount.ToString("N2", CultureInfo.InvariantCulture)}";
    }

    private static string FormatDate(DateTime? value)
    {
        return value is null || value.Value == default
            ? "-"
            : value.Value.ToString("MMM dd, yyyy", CultureInfo.InvariantCulture);
    }

    private static string FormatDateTime(DateTime? value)
    {
        return value is null || value.Value == default
            ? "-"
            : value.Value.ToString("MMM dd, yyyy hh:mm tt", CultureInfo.InvariantCulture);
    }

    private static string Html(string? value)
    {
        if (string.IsNullOrEmpty(value))
            return string.Empty;

        return value
            .Replace("&", "&amp;")
            .Replace("<", "&lt;")
            .Replace(">", "&gt;")
            .Replace("\"", "&quot;")
            .Replace("'", "&#39;");
    }

    private static string MakeSafeFileName(string value)
    {
        foreach (var invalidChar in Path.GetInvalidFileNameChars())
            value = value.Replace(invalidChar, '-');

        return value.Trim();
    }
}