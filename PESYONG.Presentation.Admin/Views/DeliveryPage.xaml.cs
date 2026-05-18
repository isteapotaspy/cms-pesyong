using System.Windows.Controls;
using CommunityToolkit.Mvvm.Input;
using PESYONG.Presentation.Admin.ViewModels.Deliveries;

namespace PESYONG.Presentation.Admin.Views;

public partial class DeliveryPage : Page
{
    private readonly DeliveryPageVM _viewModel;
    private bool _hasLoaded;

    public DeliveryPage(DeliveryPageVM viewModel)
    {
        InitializeComponent();

        _viewModel = viewModel;
        DataContext = _viewModel;

        Loaded += DeliveryPage_Loaded;
    }

    private async void DeliveryPage_Loaded(object sender, System.Windows.RoutedEventArgs e)
    {
        if (_hasLoaded)
            return;

        _hasLoaded = true;

        if (_viewModel.LoadCommand.CanExecute(null))
            await _viewModel.LoadCommand.ExecuteAsync(null);
    }
}