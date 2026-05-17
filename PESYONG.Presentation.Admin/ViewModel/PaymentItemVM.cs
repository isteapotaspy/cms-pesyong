using System;
using System.Collections.Generic;
using System.Text;
using CMS.Contracts.Admin.Payment;
using CMS.Domain.Entities.Orders;
using CMS.Domain.Enums;
using CommunityToolkit.Mvvm.ComponentModel;

namespace PESYONG.Presentation.Admin.ViewModels;

public partial class PaymentItemVM : ObservableObject
{
    [ObservableProperty]
    private int id;

    [ObservableProperty]
    private int orderId;

    [ObservableProperty]
    private string externalReference = string.Empty;

    [ObservableProperty]
    private string paymentMethod = string.Empty;

    [ObservableProperty]
    private string paymentStatus = string.Empty;

    [ObservableProperty]
    private DateTime timestampUtc = DateTime.UtcNow;

    [ObservableProperty]
    private decimal amount;

    [ObservableProperty]
    private string description = string.Empty;

    [ObservableProperty]
    private DateTime? dateCreated;

    [ObservableProperty]
    private DateTime? dateUpdated;

    [ObservableProperty]
    private string orderDisplay = string.Empty;

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
