using CMS.Contracts.Admin.Package;

namespace PESYONG.Presentation.Admin.Interfaces;

public interface IPackageApiService
{
    Task<List<PackageDto>> GetAllAsync();
    Task<PackageDto?> GetByIdAsync(int id);
    Task<PackageLookupDto> GetLookupsAsync();
    Task<PackageDto> CreateAsync(CreatePackageRequest request);
    Task<PackageDto> UpdateAsync(int id, UpdatePackageRequest request);
    Task DeleteAsync(int id);
}