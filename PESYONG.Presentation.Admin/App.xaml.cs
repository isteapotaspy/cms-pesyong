using System;
using System.Net.Http;
using System.Windows;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PESYONG.Presentation.Admin.Interfaces;
using PESYONG.Presentation.Admin.Services;

// FIX THIS INCONSISTENT DEPENDENCY ISSUE LATER ON PLS

using PESYONG.Presentation.Admin.ViewModel;
using PESYONG.Presentation.Admin.ViewModels;
using PESYONG.Presentation.Admin.ViewModels.Deliveries;
using PESYONG.Presentation.Admin.Views;

namespace PESYONG.Presentation.Admin;

// minor changes
public partial class App : Application
{
    private IHost _host = default!;

    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        _host = Host.CreateDefaultBuilder()
            .ConfigureAppConfiguration(config =>
            {
                config.SetBasePath(AppContext.BaseDirectory);
                config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            })
            .ConfigureServices((context, services) =>
            {                var apiBaseUrl = context.Configuration["ApiBaseUrl"];

                if (string.IsNullOrWhiteSpace(apiBaseUrl))
                {
                    throw new InvalidOperationException("\n\nApiBaseUrl is missing in appsettings.json.\n\n");
                }

                services.AddHttpClient("CMSApi", client =>
                {
                    client.BaseAddress = new Uri(apiBaseUrl);
                });

                services.AddScoped<IMealApiService, MealApiService>();
                services.AddTransient<MealPageVM>();
                services.AddTransient<MealsPage>();

                services.AddScoped<IDeliveryApiService, DeliveryApiService>();
                services.AddTransient<DeliveryPageVM>();
                services.AddTransient<DeliveryPage>();

                services.AddScoped<IPackageApiService, PackageApiService>();
                services.AddTransient<PackagesPageVM>();
                services.AddTransient<PackagesPage>();

                services.AddScoped<IPaymentApiService, PaymentApiService>();
                services.AddTransient<PaymentPageVM>();
                services.AddTransient<PaymentsPage>();

                services.AddScoped<IPromoApiService, PromoApiService>();
                services.AddTransient<PromosPageVM>();
                services.AddTransient<PromosPage>();               

                services.AddScoped<IOrderApiService, OrderApiService>();
                services.AddTransient<OrderPageVM>();
                services.AddTransient<OrdersPage>();

                services.AddTransient<LoginPage>();
                services.AddTransient<MainWindow>();
                services.AddTransient<MainAdminWindow>();
            })
            .Build();

        await _host.StartAsync();
        await Task.Delay(10000);
        await CheckApiConnectionAsync();

        var mainWindow = _host.Services.GetRequiredService<MainWindow>();
        mainWindow.Show();
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        if (_host is not null)
        {
            await _host.StopAsync();
            _host.Dispose();
        }

        base.OnExit(e);
    }

    private async Task CheckApiConnectionAsync()
    {
        using var scope = _host.Services.CreateScope();

        var httpClientFactory = scope.ServiceProvider.GetRequiredService<IHttpClientFactory>();
        var client = httpClientFactory.CreateClient("CMSApi");

        if (client.BaseAddress is null)
        {
            throw new InvalidOperationException("CMSApi HttpClient BaseAddress was not set.");
        }

        try
        {
            var response = await client.GetAsync("api/ping");

            if (!response.IsSuccessStatusCode)
            {
                throw new InvalidOperationException(
                    $"API was reached, but returned {(int)response.StatusCode} {response.ReasonPhrase}.");
            }
        }
        catch (HttpRequestException ex)
        {
            throw new InvalidOperationException(
                $"Cannot connect to API at {client.BaseAddress}. Make sure the ASP.NET API is running.",
                ex);
        }
    }
}