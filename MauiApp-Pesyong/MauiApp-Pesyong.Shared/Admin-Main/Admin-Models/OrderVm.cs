namespace MauiApp_Pesyong.Shared.Admin_Main.Admin_Models;

public class OrderVm
{
    public string CustomerName { get; set; } = "";
    public string DeliveryType { get; set; } = "";
    public string DeliveryStatus { get; set; } = "";
    public string Address { get; set; } = "";
    public decimal Total { get; set; }
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string PhoneNumber { get; set; } = "";
    public string Email { get; set; } = "";
    public string OrderId { get; set; } = "";
    public string CustomerId { get; set; } = "";
    public string ReceiptId { get; set; } = "";
    public DateTime OrderDate { get; set; } = DateTime.Today;
    public DateTime EstimatedDeliveryDate { get; set; } = DateTime.Today;
    public DateTime ActualDeliveryDate { get; set; } = DateTime.Today;
    public string TrackingNumber { get; set; } = "";
    public string OrderTotalText { get; set; } = "PHP 0.00";
    public string CustomerNotes { get; set; } = "";
}
