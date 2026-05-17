using System.Windows.Controls;
using PESYONG.Presentation.Admin.ViewModels;

namespace PESYONG.Presentation.Admin.Views;

public partial class PaymentsPage : Page
{
    private readonly PaymentPageVM _viewModel;
    private bool _hasLoaded;

    public PaymentsPage(PaymentPageVM viewModel)
    {
        InitializeComponent();

        _viewModel = viewModel;
        DataContext = _viewModel;

        Loaded += PaymentsPage_Loaded;
    }

    private async void PaymentsPage_Loaded(object sender, System.Windows.RoutedEventArgs e)
    {
        if (_hasLoaded)
            return;

        _hasLoaded = true;

        if (_viewModel.LoadCommand.CanExecute(null))
            await _viewModel.LoadCommand.ExecuteAsync(null);
    }
}