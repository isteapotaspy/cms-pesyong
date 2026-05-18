using System;
using System.Collections.Generic;
using System.Text;
using CMS.Contracts.Admin.Orders;

namespace PESYONG.Presentation.Admin.Interfaces;

public interface IOrderApiService
{
    Task<List<OrderDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<OrderDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<OrderDto> CreateAsync(CreateOrderRequest request, CancellationToken cancellationToken = default);
    Task<OrderDto> UpdateAsync(int id, UpdateOrderRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}