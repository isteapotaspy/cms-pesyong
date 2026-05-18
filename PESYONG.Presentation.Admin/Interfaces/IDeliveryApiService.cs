using CMS.Contracts.Admin.Deliveries;
using CMS.Contracts.Admin.Orders;

namespace PESYONG.Presentation.Admin.Interfaces;

public interface IDeliveryApiService
{
    Task<IReadOnlyList<DeliveryDto>> GetAllAsync();
    Task<DeliveryDto?> GetByIdAsync(int id);
    Task<DeliveryDto> CreateAsync(CreateDeliveryRequest request);
    Task<DeliveryDto> UpdateAsync(int id, UpdateDeliveryRequest request);
    Task DeleteAsync(int id);
}
