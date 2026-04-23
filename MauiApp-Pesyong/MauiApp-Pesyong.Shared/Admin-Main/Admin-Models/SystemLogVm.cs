namespace MauiApp_Pesyong.Shared.Admin_Main.Admin_Models;

public class SystemLogVm
{
    public string LogId { get; set; } = "";
    public DateTime Timestamp { get; set; }
    public string Level { get; set; } = "Info";
    public string Source { get; set; } = "";
    public string Message { get; set; } = "";
}
