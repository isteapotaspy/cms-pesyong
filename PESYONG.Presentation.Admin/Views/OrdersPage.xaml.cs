using System.Windows.Controls;
using PESYONG.Presentation.Admin.ViewModels;
using PESYONG.Presentation.Admin.ViewModels.Orders;

namespace PESYONG.Presentation.Admin.Views;

public partial class OrdersPage : Page
{
    private readonly OrderPageVM _viewModel;

    public OrdersPage(OrderPageVM viewModel)
    {
        InitializeComponent();

        _viewModel = viewModel;
        DataContext = _viewModel;

        Loaded += OrdersPage_Loaded;
    }

    private async void OrdersPage_Loaded(object sender, System.Windows.RoutedEventArgs e)
    {
        if (_viewModel.LoadCommand.CanExecute(null))
            await _viewModel.LoadCommand.ExecuteAsync(null);
    }
}