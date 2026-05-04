using CMS.Contracts.Customer.Orders;

namespace MauiApp_Pesyong.Shared.Customer_Main.Customer_Vms;

public class TrackingVm
{
    public string OrderId { get; set; } = string.Empty;
    public string OrderNumber { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime OrderedAt { get; set; }
    public DateTime EstimatedDeliveryTime { get; set; }

    public RiderVm Rider { get; set; } = new();
    public DeliveryAddressVm DeliveryAddress { get; set; } = new();
    public List<TrackingStepVm> Steps { get; set; } = new();
    public List<TrackingSelectionVm> Items { get; set; } = new();

    public decimal AmountPaid { get; set; }
}

public class RiderVm
{
    public string Name { get; set; } = string.Empty;
    public string Vehicle { get; set; } = string.Empty;
    public string PlateNumber { get; set; } = string.Empty;
    public decimal Rating { get; set; }
    public int TotalTrips { get; set; }
    public string ContactNumber { get; set; } = string.Empty;
}

public class TrackingStepVm
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime? Timestamp { get; set; }
    public TrackingStepState State { get; set; }
}

public class TrackingSelectionVm
{
    public string Name { get; set; } = string.Empty;
    public string Subtitle { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string ImageClass { get; set; } = "food-image-one";
}
