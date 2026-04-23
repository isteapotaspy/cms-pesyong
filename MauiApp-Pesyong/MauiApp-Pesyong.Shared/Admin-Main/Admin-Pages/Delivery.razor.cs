using Microsoft.AspNetCore.Components;
using MauiApp_Pesyong.Shared.Admin_Main.Admin_Models;
using MauiApp_Pesyong.Shared.Admin_Main.Admin_Services;

namespace MauiApp_Pesyong.Shared.Admin_Main.Admin_Pages;

public partial class Delivery : ComponentBase
{
    [Inject]
    public AdminDataService DataService { get; set; } = default!;

    protected List<DeliveryVm> deliveries = new();
    protected DeliveryVm selectedDelivery = new();
    protected string searchText = "";

    protected override void OnInitialized()
    {
        deliveries = DataService.GetDeliveries();
        selectedDelivery = CloneDelivery(deliveries.FirstOrDefault() ?? new DeliveryVm());
    }

    protected IEnumerable<DeliveryVm> FilteredDeliveries =>
        deliveries.Where(x =>
            string.IsNullOrWhiteSpace(searchText) ||
            (x.DeliveryId?.Contains(searchText, StringComparison.OrdinalIgnoreCase) ?? false) ||
            (x.DeliveryAddress?.Contains(searchText, StringComparison.OrdinalIgnoreCase) ?? false));

    protected void SelectDelivery(DeliveryVm item)
    {
        selectedDelivery = CloneDelivery(item);
    }

    protected void CreateNewDelivery()
    {
        selectedDelivery = new DeliveryVm
        {
            Status = "Pending",
            CreatedDate = DateTime.Today,
            EstimatedDelivery = DateTime.Today,
            ActualDelivery = DateTime.Today,
            ReceivedAt = DateTime.Today,
            LastLocationUpdate = DateTime.Today,
            SignatureRequired = true
        };
    }

    protected void SaveDelivery()
    {
        var existing = deliveries.FirstOrDefault(x => x.DeliveryId == selectedDelivery.DeliveryId);

        if (existing is null)
        {
            deliveries.Add(CloneDelivery(selectedDelivery));
        }
        else
        {
            existing.DeliveryId = selectedDelivery.DeliveryId;
            existing.OrderId = selectedDelivery.OrderId;
            existing.DeliveryPersonnelId = selectedDelivery.DeliveryPersonnelId;
            existing.Status = selectedDelivery.Status;
            existing.DeliveryAddress = selectedDelivery.DeliveryAddress;
            existing.TrackingNumber = selectedDelivery.TrackingNumber;
            existing.ShippingCost = selectedDelivery.ShippingCost;
            existing.ShippingMethod = selectedDelivery.ShippingMethod;
            existing.CarrierName = selectedDelivery.CarrierName;
            existing.CreatedDate = selectedDelivery.CreatedDate;
            existing.EstimatedDelivery = selectedDelivery.EstimatedDelivery;
            existing.ActualDelivery = selectedDelivery.ActualDelivery;
            existing.SignatureRequired = selectedDelivery.SignatureRequired;
            existing.ReceivedBy = selectedDelivery.ReceivedBy;
            existing.ReceivedAt = selectedDelivery.ReceivedAt;
            existing.CurrentLocation = selectedDelivery.CurrentLocation;
            existing.LastLocationUpdate = selectedDelivery.LastLocationUpdate;
            existing.SpecialInstructions = selectedDelivery.SpecialInstructions;
            existing.DeliveryNotes = selectedDelivery.DeliveryNotes;
        }
    }

    private static DeliveryVm CloneDelivery(DeliveryVm item) => new()
    {
        DeliveryId = item.DeliveryId,
        OrderId = item.OrderId,
        DeliveryPersonnelId = item.DeliveryPersonnelId,
        Status = item.Status,
        DeliveryAddress = item.DeliveryAddress,
        TrackingNumber = item.TrackingNumber,
        ShippingCost = item.ShippingCost,
        ShippingMethod = item.ShippingMethod,
        CarrierName = item.CarrierName,
        CreatedDate = item.CreatedDate,
        EstimatedDelivery = item.EstimatedDelivery,
        ActualDelivery = item.ActualDelivery,
        SignatureRequired = item.SignatureRequired,
        ReceivedBy = item.ReceivedBy,
        ReceivedAt = item.ReceivedAt,
        CurrentLocation = item.CurrentLocation,
        LastLocationUpdate = item.LastLocationUpdate,
        SpecialInstructions = item.SpecialInstructions,
        DeliveryNotes = item.DeliveryNotes
    };
}
