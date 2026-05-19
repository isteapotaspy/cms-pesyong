using System;
using CMS.Contracts.Admin.Payment;
using CommunityToolkit.Mvvm.ComponentModel;

namespace PESYONG.Presentation.Admin.ViewModel;

public partial class PaymentItemVM : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ListTitle))]
    private int id;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ListTitle))]
    private int orderId;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ListTitle))]
    private string externalReference = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ListSubtitle))]
    [NotifyPropertyChangedFor(nameof(PaymentMethodDisplay))]
    private string paymentMethod = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ListSubtitle))]
    [NotifyPropertyChangedFor(nameof(PaymentStatusDisplay))]
    private string paymentStatus = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ListSubtitle))]
    private DateTime timestampUtc = DateTime.UtcNow;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ListSubtitle))]
    private decimal amount;

    [ObservableProperty]
    private string description = string.Empty;

    [ObservableProperty]
    private DateTime? dateCreated;

    [ObservableProperty]
    private DateTime? dateUpdated;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ListTitle))]
    private string orderDisplay = string.Empty;



    public string ListTitle
    {
        get
        {
            if (!string.IsNullOrWhiteSpace(OrderDisplay))
                return OrderDisplay;

            if (!string.IsNullOrWhiteSpace(ExternalReference))
                return ExternalReference;

            if (OrderId > 0)
                return $"Order #{OrderId}";

            if (Id > 0)
                return $"Payment #{Id}";

            return "New payment";
        }
    }

    public string ListSubtitle =>
        $"{PaymentMethodDisplay} • {PaymentStatusDisplay} • {Amount:N2}";

    public string PaymentMethodDisplay =>
        string.IsNullOrWhiteSpace(PaymentMethod)
            ? "No method"
            : PaymentMethod;

    public string PaymentStatusDisplay =>
        string.IsNullOrWhiteSpace(PaymentStatus)
            ? "No status"
            : PaymentStatus;

    public static PaymentItemVM FromDto(PaymentDto dto)
    {
        return new PaymentItemVM
        {
            Id = dto.Id,
            OrderId = dto.OrderId,
            ExternalReference = dto.ExternalReference,
            PaymentMethod = dto.PaymentMethod,
            PaymentStatus = dto.PaymentStatus,
            TimestampUtc = dto.TimestampUtc,
            Amount = dto.Amount,
            Description = dto.Description,
            DateCreated = dto.DateCreated,
            DateUpdated = dto.DateUpdated,
            OrderDisplay = dto.OrderDisplay
        };
    }

    public void CopyFrom(PaymentItemVM source)
    {
        Id = source.Id;
        OrderId = source.OrderId;
        ExternalReference = source.ExternalReference;
        PaymentMethod = source.PaymentMethod;
        PaymentStatus = source.PaymentStatus;
        TimestampUtc = source.TimestampUtc;
        Amount = source.Amount;
        Description = source.Description;
        DateCreated = source.DateCreated;
        DateUpdated = source.DateUpdated;
        OrderDisplay = source.OrderDisplay;
    }

    public CreatePaymentRequest ToCreateRequest()
    {
        return new CreatePaymentRequest
        {
            OrderId = OrderId,
            ExternalReference = ExternalReference,
            PaymentMethod = PaymentMethod,
            PaymentStatus = PaymentStatus,
            TimestampUtc = TimestampUtc,
            Amount = Amount,
            Description = Description
        };
    }

    public UpdatePaymentRequest ToUpdateRequest()
    {
        return new UpdatePaymentRequest
        {
            OrderId = OrderId,
            ExternalReference = ExternalReference,
            PaymentMethod = PaymentMethod,
            PaymentStatus = PaymentStatus,
            TimestampUtc = TimestampUtc,
            Amount = Amount,
            Description = Description
        };
    }
}