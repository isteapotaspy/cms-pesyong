namespace MauiApp_Pesyong.Shared.Admin_Main.Admin_Models;

public class MealVm
{
    public string MealId { get; set; } = "";
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public decimal Price { get; set; }
    public string DeliveryType { get; set; } = "Delivery";
    public int StockQuantity { get; set; }
    public int MinOrder { get; set; }
}
