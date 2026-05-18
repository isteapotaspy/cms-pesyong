using System.Windows.Controls;
using PESYONG.Presentation.Admin.ViewModel;

namespace PESYONG.Presentation.Admin.Views;

public partial class MealsPage : Page
{
    private readonly MealPageVM _viewModel;

    public MealsPage(MealPageVM viewModel)
    {
        InitializeComponent();

        _viewModel = viewModel;
        DataContext = _viewModel;

        Loaded += MealsPage_Loaded;
    }

    private async void MealsPage_Loaded(object sender, System.Windows.RoutedEventArgs e)
    {
        if (_viewModel.LoadCommand.CanExecute(null))
        {
            await _viewModel.LoadCommand.ExecuteAsync(null);
        }
    }
}