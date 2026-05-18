using CMS.Contracts.Admin.Deliveries;
using CMS.Domain.Enums;
using CommunityToolkit.Mvvm.ComponentModel;

namespace PESYONG.Presentation.Admin.ViewModels.Deliveries;

public partial class DeliveryItemVM : ObservableObject
{
    [ObservableProperty]
    private int id;

    [ObservableProperty]
    private int orderId;

    [ObservableProperty]
    private int? riderUserId;

    [ObservableProperty]
    private DeliveryStatus status = DeliveryStatus.Pending;

    [ObservableProperty]
    private string deliveryAddressSnapshot = string.Empty;

    [ObservableProperty]
    private decimal shippingCost;

    [ObservableProperty]
    private string trackingNumber = string.Empty;

    [ObservableProperty]
    private string currentLocation = string.Empty;

    [ObservableProperty]
    private DateTime? estimatedDeliveryDateUtc;

    [ObservableProperty]
    private DateTime? actualDeliveryDateUtc;

    [ObservableProperty]
    private DateTime? createdAtUtc;

    [ObservableProperty]
    private DateTime? updatedAtUtc;

    public string DisplayName
    {
        get
        {
            var tracking = string.IsNullOrWhiteSpace(TrackingNumber)
                ? "No Tracking"
                : TrackingNumber;

            return Id == 0
                ? "New Delivery"
                : $"#{Id} - {tracking}";
        }
    }

    public static DeliveryItemVM FromDto(DeliveryDto dto)
    {
        return new DeliveryItemVM
        {
            Id = dto.Id,
            OrderId = dto.OrderId,
            RiderUserId = dto.RiderUserId,
            Status = dto.Status,
            DeliveryAddressSnapshot = dto.DeliveryAddressSnapshot,
            ShippingCost = dto.ShippingCost,
            TrackingNumber = dto.TrackingNumber,
            CurrentLocation = dto.CurrentLocation,
            EstimatedDeliveryDateUtc = dto.EstimatedDeliveryDateUtc,
            ActualDeliveryDateUtc = dto.ActualDeliveryDateUtc,
            CreatedAtUtc = dto.CreatedAtUtc,
            UpdatedAtUtc = dto.UpdatedAtUtc
        };
    }

    public void CopyFrom(DeliveryDto dto)
    {
        Id = dto.Id;
        OrderId = dto.OrderId;
        RiderUserId = dto.RiderUserId;
        Status = dto.Status;
        DeliveryAddressSnapshot = dto.DeliveryAddressSnapshot;
        ShippingCost = dto.ShippingCost;
        TrackingNumber = dto.TrackingNumber;
        CurrentLocation = dto.CurrentLocation;
        EstimatedDeliveryDateUtc = dto.EstimatedDeliveryDateUtc;
        ActualDeliveryDateUtc = dto.ActualDeliveryDateUtc;
        CreatedAtUtc = dto.CreatedAtUtc;
        UpdatedAtUtc = dto.UpdatedAtUtc;

        OnPropertyChanged(nameof(DisplayName));
    }

    public CreateDeliveryRequest ToCreateRequest()
    {
        return new CreateDeliveryRequest
        {
            OrderId = OrderId,
            RiderUserId = RiderUserId,
            Status = Status,
            DeliveryAddressSnapshot = DeliveryAddressSnapshot,
            ShippingCost = ShippingCost,
            TrackingNumber = TrackingNumber,
            CurrentLocation = CurrentLocation,
            EstimatedDeliveryDateUtc = EstimatedDeliveryDateUtc,
            ActualDeliveryDateUtc = ActualDeliveryDateUtc
        };
    }

    public UpdateDeliveryRequest ToUpdateRequest()
    {
        return new UpdateDeliveryRequest
        {
            OrderId = OrderId,
            RiderUserId = RiderUserId,
            Status = Status,
            DeliveryAddressSnapshot = DeliveryAddressSnapshot,
            ShippingCost = ShippingCost,
            TrackingNumber = TrackingNumber,
            CurrentLocation = CurrentLocation,
            EstimatedDeliveryDateUtc = EstimatedDeliveryDateUtc,
            ActualDeliveryDateUtc = ActualDeliveryDateUtc
        };
    }

    partial void OnIdChanged(int value)
    {
        OnPropertyChanged(nameof(DisplayName));
    }

    partial void OnTrackingNumberChanged(string value)
    {
        OnPropertyChanged(nameof(DisplayName));
    }
}