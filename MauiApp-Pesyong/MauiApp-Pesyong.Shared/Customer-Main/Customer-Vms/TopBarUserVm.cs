namespace MauiApp_Pesyong.Shared.Customer_Main.Customer_Vms;

public class TopBarUserVm
{
    public bool IsAuthenticated { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;

    public string AvatarLetter =>
        !string.IsNullOrWhiteSpace(FirstName)
            ? FirstName[0].ToString().ToUpperInvariant()
            : "P";
}