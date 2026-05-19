
namespace MauiApp_Pesyong.Shared.Customer_Main.Customer_Vms;

public class PickedLocationVm
{
    public decimal Latitude { get; set; }
    public decimal Longitude { get; set; }

    public string StreetAddress { get; set; } = string.Empty;
    public string Barangay { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
}