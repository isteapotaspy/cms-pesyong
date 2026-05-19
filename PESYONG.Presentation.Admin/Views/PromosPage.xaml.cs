using System.Windows;
using System.Windows.Controls;
using PESYONG.Presentation.Admin.ViewModel;

namespace PESYONG.Presentation.Admin.Views;

public partial class PromosPage : Page
{
    private bool _hasLoaded;

    public PromosPage()
    {
        InitializeComponent();
    }

    public PromosPage(PromosPageVM viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }

    private async void Page_Loaded(object sender, RoutedEventArgs e)
    {
        if (_hasLoaded)
            return;

        _hasLoaded = true;

        if (DataContext is not PromosPageVM viewModel)
            return;

        if (viewModel.LoadCommand.CanExecute(null))
        {
            await viewModel.LoadCommand.ExecuteAsync(null);
        }
    }
}