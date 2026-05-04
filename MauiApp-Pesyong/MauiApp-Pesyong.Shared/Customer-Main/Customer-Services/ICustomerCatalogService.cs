using MauiApp_Pesyong.Shared.Customer_Main.Customer_Models;

namespace MauiApp_Pesyong.Shared.Customer_Main.Customer_Services;

public interface ICustomerCatalogService
{
    Task<IReadOnlyList<string>> GetCategoriesAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PackageUiModel>> GetPackagesAsync(CancellationToken cancellationToken = default);
    Task<PackageUiModel?> GetPackageByIdAsync(string packageId, CancellationToken cancellationToken = default);
}