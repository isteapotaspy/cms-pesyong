using System;
using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using PESYONG.Presentation.Admin.Views;

namespace PESYONG.Presentation.Admin;

public partial class MainWindow : Window
{
    private readonly IServiceProvider _serviceProvider;

    public MainWindow(IServiceProvider serviceProvider)
    {
        InitializeComponent();

        _serviceProvider = serviceProvider;

        Loaded += MainWindow_Loaded;
    }

    private void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        MainFrame.Navigate(_serviceProvider.GetRequiredService<LoginPage>());
    }
}