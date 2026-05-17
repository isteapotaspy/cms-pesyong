using System;
using System.Net.Http;
using System.Windows;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PESYONG.Presentation.Admin.Services.Meals;
using PESYONG.Presentation.Admin.ViewModels.Meals;
using PESYONG.Presentation.Admin.Views;

namespace PESYONG.Presentation.Admin;

public partial class App : Application
{
    private IHost _host = default!;

    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        _host = Host.CreateDefaultBuilder()
            .ConfigureAppConfiguration(config =>
            {
                config.AddJsonFile(
                    "appsettings.json",
                    optional: true,
                    reloadOnChange: true);
            })
            .ConfigureServices((context, services) =>
            {
                var apiBaseUrl = context.Configuration["ApiBaseUrl"]
                    ?? "https://localhost:7001/";

                services.AddHttpClient("CMSApi", client =>
                {
                    client.BaseAddress = new Uri(apiBaseUrl);
                });

                services.AddTransient<IMealApiService>(sp =>
                {
                    var factory = sp.GetRequiredService<IHttpClientFactory>();
                    var httpClient = factory.CreateClient("CMSApi");

                    return new MealApiService(httpClient);
                });

                services.AddTransient<MealPageVM>();
                services.AddTransient<MealsPage>();

                services.AddTransient<LoginPage>();
                services.AddTransient<MainWindow>();
                services.AddTransient<MainAdminWindow>();
            })
            .Build();

        await _host.StartAsync();

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
}