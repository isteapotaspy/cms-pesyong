using MudBlazor;

namespace MauiApp_Pesyong.Shared.Customer_Main.Customer_Theme;

public static class CustomerTheme
{
    public static MudTheme Default => new()
    {
        PaletteLight = new PaletteLight
        {
            Primary = "#5B5FEF",
            Secondary = "#27C2B8",
            Background = "#F6F7FB",
            Surface = "#FFFFFF",
            AppbarBackground = "#FFFFFF",
            AppbarText = "#1E2230",
            DrawerBackground = "#FFFFFF",
            TextPrimary = "#1E2230",
            TextSecondary = "#6B7280",
            LinesDefault = "#E7EAF3",
            Info = "#5B5FEF",
            Success = "#22C55E",
            Warning = "#F59E0B",
            Error = "#EF4444"
        },
        LayoutProperties = new LayoutProperties
        {
            DefaultBorderRadius = "18px"
        }
    };
}