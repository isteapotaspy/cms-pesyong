using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Extensions.DependencyInjection;
using PESYONG.Presentation.Admin.Views;

namespace PESYONG.Presentation.Admin;

public partial class MainAdminWindow : Window
{
    private readonly IServiceProvider _serviceProvider;

    public MainAdminWindow(IServiceProvider serviceProvider)
    {
        InitializeComponent();

        _serviceProvider = serviceProvider;

        Loaded += MainAdminWindow_Loaded;
    }

    private void MainAdminWindow_Loaded(object sender, RoutedEventArgs e)
    {
        NavigateToPage("Dashboard");
    }

    private void NavigateMenuItem_Click(object sender, RoutedEventArgs e)
    {
        if (sender is MenuItem menuItem && menuItem.Tag is not null)
        {
            NavigateToPage(menuItem.Tag.ToString()!);
        }
    }

    private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
    {
        Application.Current.Shutdown();
    }

    private void NavigateToPage(string pageName)
    {
        try
        {
            switch (pageName)
            {
                case "Dashboard":
                    MainAdminFrame.Navigate(new DashboardPage());
                    break;

                case "Meals":
                    MainAdminFrame.Navigate(
                        _serviceProvider.GetRequiredService<MealsPage>());
                    break;

                case "Packages":
                    MainAdminFrame.Navigate(
                        _serviceProvider.GetRequiredService<PackagesPage>());
                    break;

                case "Payments":

                    MainAdminFrame.Navigate(
                        _serviceProvider.GetRequiredService<PaymentsPage>());
                    break;

                case "Promos":
                    MainAdminFrame.Navigate(
                        _serviceProvider.GetRequiredService<PromosPage>());
                    break;

                case "Orders":
                    MainAdminFrame.Navigate(
                       _serviceProvider.GetRequiredService<OrdersPage>());
                    break;

                case "Deliveries":
                    MainAdminFrame.Navigate(
                        _serviceProvider.GetRequiredService<DeliveryPage>());
                    break;

                case "Receipts":
                    MainAdminFrame.Navigate(new TextBlock
                    {
                        Text = "Receipts page not implemented yet",
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                        FontSize = 18
                    });
                    break;

                default:
                    MainAdminFrame.Navigate(new TextBlock
                    {
                        Text = $"Page '{pageName}' not implemented yet",
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                        FontSize = 18
                    });
                    break;
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading page {pageName}: {ex.Message}");
        }
    }

    private void MealsMenuItem_Click(object sender, RoutedEventArgs e)
    {
        NavigateToPage("Meals");
    }

    private void DashboardMenuItem_Click(object sender, RoutedEventArgs e)
    {
        NavigateToPage("Dashboard");
    }

    private void PacksMenuItem_Click(object sender, RoutedEventArgs e)
    {
        NavigateToPage("Packages");
    }

    private void PackagesMenuItem_Click(object sender, RoutedEventArgs e)
    {
        NavigateToPage("Packages");
    }

    private void OrdersMenuItem_Click(object sender, RoutedEventArgs e)
    {
        NavigateToPage("Orders");
    }

    private void ReceiptsMenuItem_Click(object sender, RoutedEventArgs e)
    {
        NavigateToPage("Receipts");
    }

    private void DeliveriesMenuItem_Click(object sender, RoutedEventArgs e)
    {
        NavigateToPage("Deliveries");
    }

    private void PromosMenuItem_Click(object sender, RoutedEventArgs e)
    {
        NavigateToPage("Promos");
    }

    private void PaymentsMenuItem_Click(object sender, RoutedEventArgs e)
    {
        NavigateToPage("Payments");
    }
}