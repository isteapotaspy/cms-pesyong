using System;
using System.Collections.Generic;
using System.Text;

namespace PESYONG.Presentation.Admin.Interfaces;

public interface IMealApiService
{
    Task<List<AdminMealResponse>> GetMealsAsync();
    Task<MealResponse?> GetMealByIdAsync(int id);
    Task<MealResponse> CreateMealAsync(CreateMealRequest request);
    Task<MealResponse> UpdateMealAsync(int id, UpdateMealRequest request);
    Task DeleteMealAsync(int id);
}
