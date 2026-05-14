using MauiApp_Pesyong.Shared.Customer_Main.Customer_Models;

namespace MauiApp_Pesyong.Shared.Customer_Main.Customer_Services;

public interface ICustomerCatalogService
{
    Task<IReadOnlyList<string>> GetCategoriesAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PackageUiModel>> GetPackagesAsync(CancellationToken cancellationToken = default);
    Task<PackageUiModel?> GetPackageByIdAsync(string packageId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<ShortOrderMealUiModel>> GetMealsAsync(
        string? categorySlug = null,
        string? search = null,
        CancellationToken cancellationToken = default);

    Task<ShortOrderMealUiModel?> GetMealByIdAsync(
        int mealId,
        CancellationToken cancellationToken = default);
}