using System.Windows.Controls;
using CommunityToolkit.Mvvm.Input;
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

        //_viewModel.LoadCommand is IAsyncRelayCommand loadCommand &&
        //    loadCommand.CanExecute(null)

        if (_viewModel.LoadCommand is IAsyncRelayCommand loadCommand &&
              loadCommand.CanExecute(null))
        {
            await loadCommand.ExecuteAsync(null);
        }
    }
}