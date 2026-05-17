namespace MauiApp_Pesyong.Shared.Customer_Main.Customer_Vms;

public class DeliveryAddressVm
{
    public string StreetAddress { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Barangay { get; set; } = string.Empty;
    public string Landmark { get; set; } = string.Empty;
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
}