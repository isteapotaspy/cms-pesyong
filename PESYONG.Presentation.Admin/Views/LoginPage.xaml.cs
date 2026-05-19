using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using PESYONG.Presentation.Admin.ViewModel;

namespace PESYONG.Presentation.Admin.Views;

public partial class LoginPage : Page
{
    private readonly IServiceProvider _serviceProvider;
    private readonly AdminLoginPageVM _viewModel;

    public LoginPage(IServiceProvider serviceProvider)
    {
        InitializeComponent();

        _serviceProvider = serviceProvider;
        _viewModel = _serviceProvider.GetRequiredService<AdminLoginPageVM>();

        DataContext = _viewModel;

        _viewModel.LoginSucceeded += OnLoginSucceeded;
        Unloaded += LoginPage_Unloaded;
    }

    private void AdminPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
    {
        if (DataContext is AdminLoginPageVM vm)
            vm.Password = AdminPasswordBox.Password;
    }

    private void OnLoginSucceeded(object? sender, EventArgs e)
    {
        var mainAdminWindow = _serviceProvider.GetRequiredService<MainAdminWindow>();

        mainAdminWindow.Show();

        Window.GetWindow(this)?.Close();
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        Window.GetWindow(this)?.Close();
    }

    private void LoginPage_Unloaded(object sender, RoutedEventArgs e)
    {
        _viewModel.LoginSucceeded -= OnLoginSucceeded;
        Unloaded -= LoginPage_Unloaded;
    }
}