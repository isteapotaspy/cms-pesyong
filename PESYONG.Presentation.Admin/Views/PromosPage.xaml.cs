using System.Windows;
using System.Windows.Controls;
using PESYONG.Presentation.Admin.ViewModel;

namespace PESYONG.Presentation.Admin.Views;

public partial class PromosPage : Page
{
    private readonly PromosPageVM _viewModel;
    private bool _hasLoaded;

    public PromosPage(PromosPageVM viewModel)
    {
        InitializeComponent();

        _viewModel = viewModel;
        DataContext = _viewModel;

        Loaded += PromosPage_Loaded;
    }

    private async void PromosPage_Loaded(object sender, RoutedEventArgs e)
    {
        if (_hasLoaded)
            return;

        _hasLoaded = true;

        if (_viewModel.LoadCommand.CanExecute(null))
        {
            await _viewModel.LoadCommand.ExecuteAsync(null);
        }
    }
}