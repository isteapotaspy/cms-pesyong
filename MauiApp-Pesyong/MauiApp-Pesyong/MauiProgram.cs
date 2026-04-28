using Microsoft.AspNetCore.Components.WebView.Maui;
using MauiApp_Pesyong.Shared.Services;
using MauiApp_Pesyong.Shared.Admin_Main.Admin_Services;
using MudBlazor.Services;

namespace MauiApp_Pesyong;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        builder.Services.AddMauiBlazorWebView();
        builder.Services.AddSingleton<AdminDataService>();
        builder.Services.AddMudServices();
        builder.Services.AddScoped<CustomerDrawerState>();

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
#endif

        return builder.Build();
    }
}