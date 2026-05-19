using CMS.Contracts.Admin.Address;

namespace PESYONG.Presentation.Admin.Interfaces;

public interface IAdminCustomerAddressApiService
{
    Task<CustomerAddressStringDto?> GetCustomerAddressAsync(
        int customerProfileId,
        int addressId,
        CancellationToken cancellationToken = default);
}