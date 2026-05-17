using CMS.Contracts.Admin.Meals;

namespace PESYONG.Presentation.Admin.Services.Meals;

public interface IMealApiService
{
    Task<IReadOnlyList<MealDto>> GetMealsAsync(CancellationToken cancellationToken = default);

    Task<MealDto?> GetMealByIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<MealDto> CreateMealAsync(
        CreateMealRequest request,
        CancellationToken cancellationToken = default);

    Task<MealDto> UpdateMealAsync(
        int id,
        UpdateMealRequest request,
        CancellationToken cancellationToken = default);

    Task DeleteMealAsync(
        int id,
        CancellationToken cancellationToken = default);
}