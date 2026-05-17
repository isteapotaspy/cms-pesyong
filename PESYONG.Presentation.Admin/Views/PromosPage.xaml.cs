using System.Windows.Controls;
using PESYONG.Presentation.Admin.ViewModel;
using PESYONG.Presentation.Admin.ViewModels;

namespace PESYONG.Presentation.Admin.Views;

public partial class PromosPage : Page
{
    private readonly PromosPageVM _viewModel;

    public PromosPage(PromosPageVM viewModel)
    {
        InitializeComponent();

        _viewModel = viewModel;
        DataContext = _viewModel;

        Loaded += PromosPage_Loaded;
    }

    private async void PromosPage_Loaded(object sender, System.Windows.RoutedEventArgs e)
    {
        if (_viewModel.Promos.Count == 0 && _viewModel.LoadCommand.CanExecute(null))
        {
            await _viewModel.LoadCommand.ExecuteAsync(null);
        }
    }
}