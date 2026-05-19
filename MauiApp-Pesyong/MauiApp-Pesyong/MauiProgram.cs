using MauiApp_Pesyong.Services;
using MauiApp_Pesyong.Shared.Admin_Main.Admin_Services;
using MauiApp_Pesyong.Shared.Customer_Main.Customer_Services;
using MauiApp_Pesyong.Shared.Customer_Main.Customer_Vms;
using MauiApp_Pesyong.Shared.Services;
using Microsoft.AspNetCore.Components.WebView.Maui;
using Microsoft.Extensions.DependencyInjection;
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
        builder.Services.AddScoped<ShortOrdersVm>();


        builder.Services.AddHttpClient("CMSApi", client =>
        {
            client.BaseAddress = new Uri("http://localhost:5010/");
        });

        builder.Services.AddScoped<CustomerApiClient>(sp =>
        {
            var factory = sp.GetRequiredService<IHttpClientFactory>();
            return new CustomerApiClient(factory.CreateClient("CMSApi"));
        });

        builder.Services.AddScoped<ICustomerCatalogService>(sp => sp.GetRequiredService<CustomerApiClient>());
        builder.Services.AddScoped<ICustomerOrderService>(sp => sp.GetRequiredService<CustomerApiClient>());

        builder.Services.AddScoped<ICustomerTokenStore, MauiCustomerTokenStore>();
        builder.Services.AddScoped<CustomerSession>();
        builder.Services.AddScoped<CustomerAuthHeaderHandler>();

        builder.Services.AddHttpClient<ICustomerAuthService, CustomerAuthService>(client =>
        {
            client.BaseAddress = new Uri("http://localhost:5010/");
        })
        .AddHttpMessageHandler<CustomerAuthHeaderHandler>();

        builder.Services.AddHttpClient<ICustomerCatalogService, CustomerApiClient>(client =>
        {
            client.BaseAddress = new Uri("http://localhost:5010/");
        })
        .AddHttpMessageHandler<CustomerAuthHeaderHandler>();

        builder.Services.AddHttpClient<ICustomerOrderService, CustomerApiClient>(client =>
        {
            client.BaseAddress = new Uri("http://localhost:5010/");
        })
        .AddHttpMessageHandler<CustomerAuthHeaderHandler>();

        //if testing in android emulator
        // change new Uri -> new Uri("http://10.0.2.2:5010/")

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
#endif

        return builder.Build();
    }
}