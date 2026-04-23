namespace MauiApp_Pesyong.Shared.Admin_Main.Admin_Models;

public class AppUserVm
{
    public string UserId { get; set; } = "";
    public string Username { get; set; } = "";
    public string DisplayName { get; set; } = "";
    public string Email { get; set; } = "";
    public string Role { get; set; } = "Staff";
    public bool IsActive { get; set; } = true;
    public DateTime LastLogin { get; set; } = DateTime.Today;
}
