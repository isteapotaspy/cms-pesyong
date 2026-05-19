using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using PESYONG.Presentation.Admin.ViewModels;

namespace PESYONG.Presentation.Admin.Views;

/// <summary>
/// Interaction logic for DashboardPage.xaml
/// </summary>
public partial class DashboardPage : Page
{
    private readonly DashboardPageVM _viewModel;
    private bool _hasLoaded;

    public DashboardPage(DashboardPageVM viewModel)
    {
        InitializeComponent();

        _viewModel = viewModel;
        DataContext = _viewModel;

        Loaded += DashboardPage_Loaded;
        Unloaded += DashboardPage_Unloaded;
    }

    private async void DashboardPage_Loaded(object sender, RoutedEventArgs e)
    {
        if (_hasLoaded)
            return;

        _hasLoaded = true;

        try
        {
            await _viewModel.LoadAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"Failed to load dashboard stats.\n\n{ex.Message}",
                "Dashboard Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    private async void DashboardPage_Unloaded(object sender, RoutedEventArgs e)
    {
        try
        {
            await _viewModel.DisposeAsync();
        }
        catch
        {
        }
    }
}
