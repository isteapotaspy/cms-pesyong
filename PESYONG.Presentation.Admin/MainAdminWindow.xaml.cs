using System.Windows;
using System.Windows.Controls;
using PESYONG.Presentation.Admin.Views;

namespace PESYONG.Presentation.Admin;

public partial class MainAdminWindow : Window
{
    public MainAdminWindow()
    {
        InitializeComponent();
    }

    private void MainAdminWindow_Loaded(object sender, RoutedEventArgs e)
    {
        // Load default page
        NavigateToPage("Dashboard");
    }

    private void NavigateMenuItem_Click(object sender, RoutedEventArgs e)
    {
        var menuItem = sender as MenuItem;
        if (menuItem?.Tag != null)
        {
            string pageName = menuItem?.Tag?.ToString();
            NavigateToPage(pageName);
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
                    MainAdminFrame.Navigate(new MealsPage());
                    break;
                case "Packages":
                    MainAdminFrame.Navigate(new PackagesPage());
                    break;
                case "Payments":
                    MainAdminFrame.Navigate(new PaymentsPage());
                    break;
                case "Promos":
                    MainAdminFrame.Navigate(new PromosPage());
                    break;
                case "Orders":
                    MainAdminFrame.Navigate(new OrdersPage());
                    break;
                case "Deliveries":
                    MainAdminFrame.Navigate(new DeliveryPage());
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
        catch (System.Exception ex)
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
        NavigateToPage("Packs");
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