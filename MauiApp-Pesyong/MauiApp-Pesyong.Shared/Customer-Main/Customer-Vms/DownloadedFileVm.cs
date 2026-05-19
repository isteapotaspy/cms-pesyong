
namespace MauiApp_Pesyong.Shared.Customer_Main.Customer_Vms;

public class DownloadedFileVm
{
    public string FileName { get; set; } = string.Empty;
    public string ContentType { get; set; } = "application/octet-stream";
    public byte[] Content { get; set; } = Array.Empty<byte>();
}