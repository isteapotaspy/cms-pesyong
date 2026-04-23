namespace MauiApp_Pesyong.Shared.Admin_Main.Admin_Models;

public class CustomerVm
{
    public string CustomerId { get; set; } = "";
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string Email { get; set; } = "";
    public string Phone { get; set; } = "";
    public string Address { get; set; } = "";
    public string Notes { get; set; } = "";
    public DateTime CreatedDate { get; set; } = DateTime.Today;
    public string FullName => $"{FirstName} {LastName}".Trim();
}
