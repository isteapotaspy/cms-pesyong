using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;

namespace PESYONG.Presentation.Admin.Views;

public partial class LoginPage : Page
{
    private readonly IServiceProvider _serviceProvider;

    public LoginPage(IServiceProvider serviceProvider)
    {
        InitializeComponent();

        _serviceProvider = serviceProvider;
    }

    private void LoginButton_Click(object sender, RoutedEventArgs e)
    {
        // If authentication is successful
        var mainAdminWindow = _serviceProvider.GetRequiredService<MainAdminWindow>();

        mainAdminWindow.Show();

        Window.GetWindow(this)?.Close();
    }
}