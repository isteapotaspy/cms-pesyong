namespace MauiApp_Pesyong.Shared.Admin_Main.Admin_Models;

public class DeliveryVm
{
    public string DeliveryId { get; set; } = "";
    public string OrderId { get; set; } = "";
    public string DeliveryPersonnelId { get; set; } = "";
    public string Status { get; set; } = "Pending";
    public string DeliveryAddress { get; set; } = "";
    public string TrackingNumber { get; set; } = "";
    public decimal ShippingCost { get; set; }
    public string ShippingMethod { get; set; } = "";
    public string CarrierName { get; set; } = "";
    public DateTime CreatedDate { get; set; }
    public DateTime EstimatedDelivery { get; set; }
    public DateTime ActualDelivery { get; set; }
    public bool SignatureRequired { get; set; } = true;
    public string ReceivedBy { get; set; } = "";
    public DateTime ReceivedAt { get; set; }
    public string CurrentLocation { get; set; } = "";
    public DateTime LastLocationUpdate { get; set; }
    public string SpecialInstructions { get; set; } = "";
    public string DeliveryNotes { get; set; } = "";
}
