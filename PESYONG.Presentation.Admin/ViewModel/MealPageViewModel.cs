using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace PESYONG.Presentation.Admin.ViewModel;

public partial class MealsPageViewModel : ObservableValidator
{
    private readonly IMealService _mealService;

    public MealsPageViewModel(IMealService mealService)
    {
        _mealService = mealService;
    }

    [ObservableProperty]
    private ObservableCollection<MealItemVm> meals = new();

    [ObservableProperty]
    private MealItemVm? selectedMeal;

    [ObservableProperty]
    private bool isLoading;

    [ObservableProperty]
    private string searchText = string.Empty;

    [RelayCommand]
    private async Task LoadMealsAsync()
    {
        IsLoading = true;

        try
        {
            var result = await _mealService.GetMealsAsync();

            Meals.Clear();

            foreach (var meal in result)
            {
                Meals.Add(new MealItemVm
                {
                    MealId = meal.Id,
                    Name = meal.Name,
                    Description = meal.Description,
                    Price = meal.Price,
                    ImageUrl = meal.ImageUrl,
                    IsAvailable = meal.IsAvailable
                });
            }
        }
        finally
        {
            IsLoading = false;
        }
    }
}