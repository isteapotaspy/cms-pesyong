using System.Windows.Controls;
using CommunityToolkit.Mvvm.Input;
using PESYONG.Presentation.Admin.ViewModel;

namespace PESYONG.Presentation.Admin.Views;

public partial class PackagesPage : Page
{
    private readonly PackagesPageVM _viewModel;

    public PackagesPage(PackagesPageVM viewModel)
    {
        InitializeComponent();

        _viewModel = viewModel;
        DataContext = _viewModel;

        Loaded += PackagesPage_Loaded;
    }

    private async void PackagesPage_Loaded(object sender, System.Windows.RoutedEventArgs e)
    {
        if (_viewModel.LoadCommand is IAsyncRelayCommand loadCommand &&
            loadCommand.CanExecute(null))
        {
            await loadCommand.ExecuteAsync(null);
        }
    }
}