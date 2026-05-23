using MauiApp_Pesyong.Services;
using MauiApp_Pesyong.Shared.Admin_Main.Admin_Services;
using MauiApp_Pesyong.Shared.Customer_Main.Customer_Services;
using MauiApp_Pesyong.Shared.Customer_Main.Customer_Vms;
using MauiApp_Pesyong.Shared.Services;
using Microsoft.AspNetCore.Components.WebView.Maui;
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

        const string apiBaseUrl = "http://10.8.51.195:5010/";
        // Android emulator: http://10.0.2.2:5010/
        // Windows: http://localhost:5010/
        // LAN: http://192.168.100.246:5010/

        builder.Services.AddMauiBlazorWebView();
        builder.Services.AddMudServices();

        builder.Services.AddSingleton<AdminDataService>();

        builder.Services.AddScoped<CustomerDrawerState>();
        builder.Services.AddScoped<ShortOrdersVm>();
        builder.Services.AddScoped<CustomerSession>();
        builder.Services.AddScoped<ICustomerTokenStore, MauiCustomerTokenStore>();
        builder.Services.AddScoped<CustomerAuthHeaderHandler>();

        // Plain named client used by SignalR realtime service for base URL
        builder.Services.AddHttpClient("CMSApi", client =>
        {
            client.BaseAddress = new Uri(apiBaseUrl);
        });

        // CustomerApiClient now handles bearer tokens itself for protected calls
        builder.Services.AddHttpClient<CustomerApiClient>(client =>
        {
            client.BaseAddress = new Uri(apiBaseUrl);
        });

        builder.Services.AddScoped<ICustomerCatalogService>(sp => sp.GetRequiredService<CustomerApiClient>());
        builder.Services.AddScoped<ICustomerOrderService>(sp => sp.GetRequiredService<CustomerApiClient>());

        builder.Services.AddHttpClient<ICustomerAuthService, CustomerAuthService>(client =>
        {
            client.BaseAddress = new Uri(apiBaseUrl);
        });

        builder.Services.AddScoped<ICustomerOrderRealtimeService, CustomerOrderRealtimeService>();

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
#endif

        return builder.Build();
    }
}